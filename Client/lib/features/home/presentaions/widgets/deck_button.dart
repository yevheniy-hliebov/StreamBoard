import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:streamtabula/core/constants/app_colors.dart';
import 'package:streamtabula/features/home/data/models/deck_button_data.dart';

class DeckButton extends StatelessWidget {
  final Size size;
  final String keyCode;
  final DeckButtonData? data;
  final void Function(String keyCode)? onTap;

  final Future<Uint8List?> Function(String keyCode, String? imagePath)?
  getImage;

  const DeckButton({
    super.key,
    this.size = const Size(50, 50),
    required this.keyCode,
    this.data,
    this.onTap,
    this.getImage,
  });

  @override
  Widget build(BuildContext context) {
    String label = keyCode;
    if (data != null && data!.name.isNotEmpty) {
      label = data!.name;
    }

    return Tooltip(
      message: label,
      child: InkWell(
        onTap: () {
          onTap?.call(keyCode);
        },
        child: Container(
          width: size.width,
          height: size.height,
          clipBehavior: Clip.antiAlias,
          decoration: BoxDecoration(
            color: data?.backgroundColor,
            borderRadius: BorderRadius.circular(8),
          ),
          // Рамка малюється ПОВЕРХ картинки
          foregroundDecoration: BoxDecoration(
            borderRadius: BorderRadius.circular(8),
            border: Border.all(color: AppColors.of(context).outline, width: 1),
          ),
          child: _buildImage(),
        ),
      ),
    );
  }

  Widget _buildImage() {
    if (data == null || data!.imagePath.isEmpty) {
      return const SizedBox.shrink();
    }

    return FutureBuilder<Uint8List?>(
      future: getImage?.call(keyCode, data!.imagePath),
      builder: (context, snapshot) {
        if (!snapshot.hasData || snapshot.data == null) {
          return const SizedBox.shrink();
        }

        return Image.memory(
          snapshot.data!,
          fit: BoxFit.cover,
          width: double.infinity,
          height: double.infinity,
        );
      },
    );
  }
}
