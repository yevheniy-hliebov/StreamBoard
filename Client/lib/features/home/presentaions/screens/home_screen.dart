import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:streamboard/common/widgets/orientation_adaptive_scaffold.dart';
import 'package:streamboard/core/constants/spacing.dart';
import 'package:streamboard/features/home/presentaions/widgets/grid_deck_layout.dart';
import 'package:streamboard/features/home/providers/deck_provider.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return OrientationAdaptiveScaffold(
      title: 'StreamBoard',
      leading: IconButton(
        icon: const Icon(Icons.refresh),
        onPressed: () {
          context.read<DeckProvider>().fetchDeck();
        },
      ),
      actions: [IconButton(icon: const Icon(Icons.settings), onPressed: () {})],
      body: Padding(
        padding: const EdgeInsets.all(Spacing.sm),
        child: Consumer<DeckProvider>(
          builder: (context, provider, child) {
            if (provider.isLoading) {
              return const Center(child: CircularProgressIndicator());
            }

            if (provider.error != null) {
              return Center(child: Text('Error: ${provider.error}'));
            }

            if (provider.gridLayout == null) {
              return const Center(child: Text('No layout data'));
            }

            return GridDeckLayout(
              grid: provider.gridLayout!,
              buttons: provider.buttons,
              getImage: provider.getImage,
              onTap: provider.clickButton,
            );
          },
        ),
      ),
    );
  }
}
