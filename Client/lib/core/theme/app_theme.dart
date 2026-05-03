import 'package:flutter/material.dart';
import 'package:streamboard/core/constants/app_colors.dart';

class AppTheme {
  const AppTheme._();

  static ThemeData light = _buildTheme(AppColors.light, Brightness.light);
  static ThemeData dark = _buildTheme(AppColors.dark, Brightness.dark);

  static ThemeData _buildTheme(AppColorsData colors, Brightness brightness) {
    return ThemeData(
      colorScheme: ColorScheme.fromSwatch().copyWith(
        primary: colors.primary,
        surface: colors.surface,
        onSurface: colors.onSurface,
        outline: colors.outline,
        brightness: brightness,
      ),
      scaffoldBackgroundColor: colors.background,
      iconTheme: IconThemeData(color: colors.onBackground),
      listTileTheme: ListTileThemeData(
        iconColor: colors.onBackground,
        subtitleTextStyle: TextStyle().copyWith(color: colors.onBackground),
      ),
      dialogTheme: DialogThemeData(backgroundColor: colors.background),
    );
  }
}
