import 'package:flutter/material.dart';
import 'package:streamboard/core/utils/color_helper.dart';

class DeckButtonData {
  final String keyCode;
  final String name;
  final Color backgroundColor;
  final String imagePath;

  const DeckButtonData({
    this.keyCode = '',
    this.name = '',
    this.backgroundColor = Colors.transparent,
    this.imagePath = '',
  });

  factory DeckButtonData.fromJson(String keyCode, Map<String, dynamic> json) {
    return DeckButtonData(
      keyCode: keyCode,
      name: json["name"] ?? '',
      backgroundColor:
          ColorHelper.hexToColor(json["background_color"]) ??
          Colors.transparent,
      imagePath: json["image_path"] ?? '',
    );
  }
}
