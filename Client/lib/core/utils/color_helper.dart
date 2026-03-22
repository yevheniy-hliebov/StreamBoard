import 'package:flutter/material.dart';

class ColorHelper {
  static Color? hexToColor(String? hexString) {
    if (hexString == '' || hexString == null) return null;

    hexString = hexString.toUpperCase().replaceAll('#', '');

    if (hexString.length == 6) {
      hexString = 'FF$hexString';
    }

    return Color(int.parse(hexString, radix: 16));
  }
}
