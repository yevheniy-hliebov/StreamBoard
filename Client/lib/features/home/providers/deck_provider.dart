import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:streamboard/features/home/data/models/deck_button_data.dart';
import 'package:streamboard/features/home/data/models/grid_layout.dart';
import 'package:streamboard/features/home/services/grid_service.dart';

class DeckProvider extends ChangeNotifier {
  GridService _service;

  GridLayout? gridLayout;
  Map<String, DeckButtonData> buttons = {};
  bool isLoading = true;
  String? error;

  DeckProvider(this._service) {
    fetchDeck();
  }

  Future<void> fetchDeck() async {
    isLoading = true;
    error = null;
    notifyListeners();

    try {
      final data = await _service.getButtons();

      gridLayout = GridLayout.fromJson(data['grid_layout']);

      final pageMap = data['page_map'] as Map<String, dynamic>;
      buttons = pageMap.map(
        (key, value) => MapEntry(key, DeckButtonData.fromJson(key, value)),
      );
    } catch (e) {
      error = e.toString();
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  Future<Uint8List?> getImage(String keyCode) async {
    try {
      return await _service.getImage(keyCode);
    } catch (e) {
      return null;
    }
  }

  Future<void> clickButton(String keyCode) async {
    try {
      await _service.clickButton(keyCode);
    } catch (e) {
      debugPrint('Error clicking button: $e');
    }
  }

  void updateService(GridService newService) {
    if (_service.baseUrl != newService.baseUrl) {
      _service = newService;
      fetchDeck();
    } else {
      _service = newService;
    }
  }
}
