import 'package:flutter/material.dart';
import 'package:streamboard/core/constants/spacing.dart';

class SetupConnectionPrompt extends StatelessWidget {
  final VoidCallback onSetupPressed;

  const SetupConnectionPrompt({super.key, required this.onSetupPressed});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.wifi_off_rounded,
            size: 64,
            color: Theme.of(context).colorScheme.onSurfaceVariant,
          ),
          const SizedBox(height: Spacing.md),
          Text(
            'Connection Not Configured',
            style: Theme.of(context).textTheme.titleLarge,
          ),
          const SizedBox(height: Spacing.sm),
          Text(
            'Please specify the StreamBoard server address to connect.',
            style: Theme.of(context).textTheme.bodyMedium,
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: Spacing.lg),
          FilledButton.icon(
            onPressed: onSetupPressed,
            icon: const Icon(Icons.settings),
            label: const Text('Go to Settings'),
          ),
        ],
      ),
    );
  }
}
