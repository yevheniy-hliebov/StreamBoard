import 'dart:math';
import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:streamboard/common/widgets/for.dart';
import 'package:streamboard/core/constants/spacing.dart';
import 'package:streamboard/features/home/data/models/deck_button_data.dart';
import 'package:streamboard/features/home/data/models/grid_layout.dart';
import 'package:streamboard/features/home/presentaions/widgets/deck_button.dart';

class GridDeckLayout extends StatelessWidget {
  final GridLayout grid;
  final Map<String, DeckButtonData> buttons;
  final Future<Uint8List?> Function(String keyCode)? getImage;
  final void Function(String keyCode)? onTap;

  const GridDeckLayout({
    super.key,
    required this.grid,
    this.buttons = const {},
    this.getImage,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final isPortrait =
            MediaQuery.of(context).orientation == Orientation.portrait;

        final rows = isPortrait ? grid.columns : grid.rows;
        final columns = isPortrait ? grid.rows : grid.columns;

        final spacing = Spacing.keyGrid.btwKey;

        final totalRowGaps = (columns - 1) * spacing;
        final width = (constraints.maxWidth - totalRowGaps) / columns;

        final totalColumnGaps = (rows - 1) * spacing;
        final height =
            ((constraints.maxHeight - totalColumnGaps) / rows) - Spacing.xxs;

        final buttonSize = Size.square(min(width, height));

        return Row(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Column(
              spacing: spacing,
              mainAxisAlignment: MainAxisAlignment.center,
              crossAxisAlignment: CrossAxisAlignment.center,
              children: For.generateChildren(
                rows,
                generator: (rowIndex) => [
                  Row(
                    spacing: spacing,
                    children: For.generateChildren(
                      columns,
                      generator: (colIndex) {
                        final itemIndex = rowIndex * columns + colIndex;
                        final keyCode = itemIndex.toString();
                        return [
                          DeckButton(
                            size: buttonSize,
                            keyCode: keyCode,
                            data: buttons[keyCode],
                            getImage: getImage,
                            onTap: onTap,
                          ),
                        ];
                      },
                    ),
                  ),
                ],
              ),
            ),
          ],
        );
      },
    );
  }
}
