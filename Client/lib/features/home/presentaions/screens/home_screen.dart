import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:streamboard/common/widgets/orientation_adaptive_scaffold.dart';
import 'package:streamboard/core/constants/spacing.dart';
import 'package:streamboard/features/home/presentaions/widgets/grid_deck_layout.dart';
import 'package:streamboard/features/home/presentaions/widgets/setup_connection_prompt.dart';
import 'package:streamboard/features/home/providers/deck_provider.dart';
import 'package:streamboard/features/settings/presentations/screens/settings_screen.dart';
import 'package:streamboard/features/settings/providers/settings_provider.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final isConfigured = context.watch<SettingsProvider>().isConfigured;

    return OrientationAdaptiveScaffold(
      title: 'StreamBoard',
      leading: IconButton(
        icon: const Icon(Icons.refresh),
        onPressed: isConfigured
            ? () => context.read<DeckProvider>().fetchDeck()
            : null,
      ),
      actions: [
        IconButton(
          icon: const Icon(Icons.settings),
          onPressed: () => _openSettings(context),
        ),
      ],
      body: Padding(
        padding: const EdgeInsets.all(Spacing.sm),
        child: !isConfigured
            ? SetupConnectionPrompt(
                onSetupPressed: () => _openSettings(context),
              )
            : _buildDeckGrid(context),
      ),
    );
  }

  void _openSettings(BuildContext context) {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const SettingsScreen()),
    );
  }

  Widget _buildDeckGrid(BuildContext context) {
    return Consumer<DeckProvider>(
      builder: (context, provider, child) {
        if (provider.isLoading) {
          return const Center(child: CircularProgressIndicator());
        }

        if (provider.error != null) {
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  'Connection Error',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                const SizedBox(height: Spacing.xs),
                Text(provider.error!, textAlign: TextAlign.center),
                const SizedBox(height: Spacing.md),
                OutlinedButton.icon(
                  onPressed: () => _openSettings(context),
                  icon: const Icon(Icons.settings),
                  label: const Text('Check Settings'),
                ),
              ],
            ),
          );
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
    );
  }
}
