import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

class SettingsProvider extends ChangeNotifier {
  final SharedPreferences _prefs;

  late String address;
  late String port;
  late bool isConfigured;

  SettingsProvider(this._prefs) {
    isConfigured = _prefs.containsKey('server_address');

    address = _prefs.getString('server_address') ?? '';
    port = _prefs.getString('server_port') ?? '13550';
  }

  String get baseUrl =>
      'http://${address.isEmpty ? "localhost" : address}:$port';

  Future<void> saveSettings(String newAddress, String newPort) async {
    address = newAddress;
    port = newPort;
    isConfigured = true;

    await _prefs.setString('server_address', address);
    await _prefs.setString('server_port', port);

    notifyListeners();
  }
}
