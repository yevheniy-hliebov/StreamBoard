import 'package:flutter/material.dart';
import 'package:streamboard/common/widgets/orientation_adaptive_scaffold.dart';
import 'package:streamboard/core/constants/spacing.dart';
import 'package:streamboard/features/home/data/models/grid_layout.dart';
import 'package:streamboard/features/home/presentaions/widgets/grid_deck_layout.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return OrientationAdaptiveScaffold(
      title: 'StreamBoard',
      leading: IconButton(icon: const Icon(Icons.refresh), onPressed: () {}),
      actions: [IconButton(icon: const Icon(Icons.settings), onPressed: () {})],
      body: Padding(
        padding: const EdgeInsets.all(Spacing.sm),
        child: GridDeckLayout(
          grid: GridLayout(name: '3x2', columns: 6, rows: 3),
        ),
      ),
    );
  }
}
