import 'dart:async';
import 'dart:convert';
import 'dart:math';
import 'package:flutter/foundation.dart';
import 'package:web_socket_channel/web_socket_channel.dart';

class WebSocketService {
  WebSocketChannel? _channel;

  final _messageController = StreamController<Map<String, dynamic>>.broadcast();
  final _connectionStateController = StreamController<bool>.broadcast();

  Timer? _reconnectTimer;
  int _reconnectAttempts = 0;
  bool _isIntentionalDisconnect = false;
  String? _currentUrl;

  bool _isConnected = false;

  Stream<Map<String, dynamic>> get messages => _messageController.stream;
  Stream<bool> get connectionState => _connectionStateController.stream;
  bool get isConnected => _isConnected;

  void _setConnectionState(bool state) {
    if (_isConnected != state) {
      _isConnected = state;
      _connectionStateController.add(state);
    }
  }

  void connect(String baseUrl) {
    _isIntentionalDisconnect = false;

    final wsUrl =
        '${baseUrl.replaceFirst('http://', 'ws://').replaceFirst('https://', 'wss://')}/ws';

    _currentUrl = wsUrl;
    _connectInternal();
  }

  void _connectInternal() async {
    if (_currentUrl == null) return;

    try {
      debugPrint('Connecting to WebSocket: $_currentUrl');
      final uri = Uri.parse(_currentUrl!);
      _channel = WebSocketChannel.connect(uri);

      await _channel!.ready;

      _setConnectionState(true);

      _channel!.stream.listen(
        (message) {
          _reconnectAttempts = 0;
          try {
            final decoded = jsonDecode(message as String);
            _messageController.add(decoded);
          } catch (e) {
            debugPrint('WebSocket JSON parse error: $e');
          }
        },
        onDone: () {
          debugPrint('WebSocket closed');
          _setConnectionState(false);
          _scheduleReconnect();
        },
        onError: (error) {
          debugPrint('WebSocket error: $error');
          _setConnectionState(false);
          _scheduleReconnect();
        },
        cancelOnError: true,
      );
    } catch (e) {
      debugPrint('WebSocket connection exception: $e');
      _setConnectionState(false);
      _scheduleReconnect();
    }
  }

  void _scheduleReconnect() {
    if (_isIntentionalDisconnect) return;

    _channel?.sink.close();
    _channel = null;

    final delay = min(pow(2, _reconnectAttempts) * 1000, 30000).toInt();
    _reconnectAttempts++;

    debugPrint(
      'Reconnecting in ${delay / 1000} seconds... (Attempt $_reconnectAttempts)',
    );

    _reconnectTimer?.cancel();
    _reconnectTimer = Timer(Duration(milliseconds: delay), _connectInternal);
  }

  void reconnect() {
    debugPrint('Manual reconnect triggered...');
    _reconnectTimer?.cancel();
    _channel?.sink.close();
    _channel = null;

    _reconnectAttempts = 0;
    _isIntentionalDisconnect = false;
    _setConnectionState(false);

    if (_currentUrl != null) {
      _connectInternal();
    }
  }

  void send(String type, [Map<String, dynamic>? data]) {
    if (_channel != null) {
      final payload = jsonEncode({'type': type, 'data': ?data});
      _channel!.sink.add(payload);
    } else {
      debugPrint('Cannot send message: WebSocket is not connected');
    }
  }

  void disconnect() {
    _isIntentionalDisconnect = true;
    _reconnectTimer?.cancel();
    _channel?.sink.close();
    _channel = null;
    _setConnectionState(false);
    debugPrint('WebSocket intentionally disconnected');
  }

  void dispose() {
    disconnect();
    _messageController.close();
    _connectionStateController.close();
  }
}
