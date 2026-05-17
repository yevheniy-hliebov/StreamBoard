import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

class SettingsProvider extends ChangeNotifier {
  final SharedPreferences _prefs;

  late String address;
  late String port;
  late bool isConfigured;
  late bool receiveBetaUpdates;
  late String skippedVersion;

  SettingsProvider(this._prefs) {
    isConfigured = _prefs.containsKey('server_address');

    address = _prefs.getString('server_address') ?? '';
    port = _prefs.getString('server_port') ?? '13550';
    receiveBetaUpdates = _prefs.getBool('receive_beta_updates') ?? false;
    skippedVersion = _prefs.getString('skipped_version') ?? '';
  }

  String get baseUrl =>
      'http://${address.isEmpty ? "localhost" : address}:$port';

  Future<void> saveServerSettings(String newAddress, String newPort) async {
    address = newAddress;
    port = newPort;
    isConfigured = true;

    await _prefs.setString('server_address', address);
    await _prefs.setString('server_port', port);

    notifyListeners();
  }

  Future<void> updateServerAddress(String rawUrl) async {
    try {
      final uri = Uri.parse(rawUrl);
      final newAddress = uri.host;

      final newPort = uri.hasPort ? uri.port.toString() : port;

      if (newAddress.isNotEmpty) {
        await saveServerSettings(newAddress, newPort);
      }
    } catch (e) {
      debugPrint('Error parsing URL from QR code: $e');
    }
  }

  Future<void> toggleBetaUpdates(bool value) async {
    receiveBetaUpdates = value;
    await _prefs.setBool('receive_beta_updates', value);
    notifyListeners();
  }

  Future<void> saveSkippedVersion(String version) async {
    skippedVersion = version;
    await _prefs.setString('skipped_version', version);
    notifyListeners();
  }
}
