import 'dart:async';
import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:streamtabula/features/home/data/models/deck_button_data.dart';
import 'package:streamtabula/features/home/data/models/grid_layout.dart';
import 'package:streamtabula/features/home/services/grid_service.dart';
import 'package:streamtabula/features/home/services/websocket_service.dart';

class DeckProvider extends ChangeNotifier {
  GridService _gridService;
  WebSocketService _wsService;
  StreamSubscription? _wsSubscription;

  final Map<String, Uint8List> _imageCache = {};

  GridLayout? gridLayout;
  Map<String, DeckButtonData> buttons = {};
  String currentPageName = 'StreamTabula';
  bool isLoading = true;
  String? error;

  DeckProvider(this._gridService, this._wsService) {
    fetchDeck();
    _initWebSocket();
  }

  void _initWebSocket() {
    _wsSubscription = _wsService.messages.listen((message) {
      final type = message['type'] as String?;
      final data = message['data'] as Map<String, dynamic>?;

      if (type == null || data == null) return;

      switch (type) {
        case 'ButtonAppearanceChanged':
          _handleButtonUpdated(data);
          break;
        case 'ButtonsSwapped':
          _handleButtonsSwapped(data);
          break;
        case 'PageRenamed':
          _handlePageRenamed(data);
          break;
        case 'PageChanged':
        case 'GridLayoutChanged':
          fetchDeck(showLoading: false);
          break;
      }
    });
  }

  void _handleButtonUpdated(Map<String, dynamic> data) {
    final String keyCode = data['index'].toString();

    buttons[keyCode] = DeckButtonData.fromJson(keyCode, data);

    buttons = Map.of(buttons);
    notifyListeners();
  }

  void _handleButtonsSwapped(Map<String, dynamic> data) {
    final String keyA = data['index_a'].toString();
    final String keyB = data['index_b'].toString();

    final buttonA = buttons[keyA];
    final buttonB = buttons[keyB];

    if (buttonB != null) {
      buttons[keyA] = buttonB.copyWith(keyCode: keyA);
    } else {
      buttons.remove(keyA);
    }

    if (buttonA != null) {
      buttons[keyB] = buttonA.copyWith(keyCode: keyB);
    } else {
      buttons.remove(keyB);
    }

    buttons = Map.of(buttons);
    notifyListeners();
  }

  void _handlePageRenamed(Map<String, dynamic> data) {
    if (data['pageName'] != null) {
      currentPageName = data['pageName'];
      notifyListeners();
    }
  }

  Future<void> fetchDeck({bool showLoading = true}) async {
    if (showLoading) {
      isLoading = true;
      error = null;
      notifyListeners();
    }

    try {
      final data = await _gridService.getButtons();

      gridLayout = GridLayout.fromJson(data['grid_layout']);

      currentPageName = data['current_page_name'] ?? 'StreamTabula';

      final pageMap = data['page_map'] as Map<String, dynamic>? ?? {};
      buttons = pageMap.map(
        (key, value) => MapEntry(key, DeckButtonData.fromJson(key, value)),
      );
    } catch (e) {
      error = e.toString();
    } finally {
      // Завжди вимикаємо лоадер в кінці
      isLoading = false;
      notifyListeners();
    }
  }

  Future<Uint8List?> getImage(String keyCode, String? imagePath) async {
    if (imagePath == null || imagePath.isEmpty) return null;

    if (_imageCache.containsKey(imagePath)) {
      return _imageCache[imagePath];
    }

    try {
      final bytes = await _gridService.getImage(keyCode);

      if (bytes != null) {
        _imageCache[imagePath] = bytes;
      }

      return bytes;
    } catch (e) {
      return null;
    }
  }

  Future<void> clickButton(String keyCode) async {
    try {
      await _gridService.clickButton(keyCode);
    } catch (e) {
      debugPrint('Error clicking button: $e');
    }
  }

  void updateServices(
    GridService newGridService,
    WebSocketService newWsService,
  ) {
    bool shouldFetch = false;

    if (_gridService.baseUrl != newGridService.baseUrl) {
      _gridService = newGridService;
      _imageCache.clear();
      shouldFetch = true;
    }

    _wsService = newWsService;

    if (shouldFetch) fetchDeck();
  }

  @override
  void dispose() {
    _wsSubscription?.cancel();
    super.dispose();
  }
}
