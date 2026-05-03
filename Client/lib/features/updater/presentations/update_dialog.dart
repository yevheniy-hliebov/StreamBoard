import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:streamboard/features/settings/providers/settings_provider.dart';
import 'package:streamboard/features/updater/providers/updater_provider.dart';

class UpdateDialog extends StatelessWidget {
  const UpdateDialog({super.key});

  @override
  Widget build(BuildContext context) {
    final provider = context.watch<UpdaterProvider>();

    return AlertDialog(
      title: const Text('Check for Updates'),
      content: SizedBox(
        width: 350,
        child: _buildDialogContent(context, provider),
      ),
    );
  }

  Widget _buildDialogContent(BuildContext context, UpdaterProvider provider) {
    switch (provider.dialogState) {
      case UpdateDialogState.loading:
        return Column(
          mainAxisAlignment: .center,
          mainAxisSize: .min,
          children: const [
            CircularProgressIndicator(),
            SizedBox(height: 16),
            Text('Checking for updates...', style: TextStyle(fontSize: 16)),
          ],
        );

      case UpdateDialogState.upToDate:
        return Column(
          mainAxisAlignment: .center,
          mainAxisSize: .min,
          children: [
            const Icon(Icons.check_circle, color: Colors.green, size: 48),
            const SizedBox(height: 16),
            const Text(
              'No new updates available',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            const Text(
              'You are currently running the latest version.',
              textAlign: TextAlign.center,
              style: TextStyle(color: Colors.grey),
            ),
            const SizedBox(height: 16),
            FilledButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('OK'),
            ),
          ],
        );

      case UpdateDialogState.updateAvailable:
        return Column(
          mainAxisAlignment: .center,
          mainAxisSize: .min,
          children: [
            const Icon(Icons.cloud_download, size: 48),
            const SizedBox(height: 16),
            Text(
              'New version available: ${provider.latestReleaseInfo?.tagName}',
              style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            const Text(
              'Would you like to install the new version now?',
              textAlign: TextAlign.center,
              style: TextStyle(color: Colors.grey),
            ),
            const SizedBox(height: 16),
            OverflowBar(
              alignment: .center,
              overflowAlignment: .center,
              children: [
                FilledButton(
                  onPressed: () => provider.startUpdateProcess(),
                  child: const Text('Update now'),
                ),
                const SizedBox(width: 8),
                TextButton(
                  onPressed: () {
                    final settings = context.read<SettingsProvider>();
                    settings.saveSkippedVersion(
                      provider.latestReleaseInfo!.tagName,
                    );
                    Navigator.pop(context);
                  },
                  child: const Text('Skip'),
                ),
                const SizedBox(width: 8),
                TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: const Text('Later'),
                ),
              ],
            ),
          ],
        );

      case UpdateDialogState.downloading:
        return Column(
          mainAxisAlignment: .center,
          crossAxisAlignment: .stretch,
          mainAxisSize: .min,
          children: [
            const Icon(Icons.downloading, size: 48),
            const SizedBox(height: 16),
            const Text(
              'Updating StreamBoard...',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            Text(
              provider.downloadStatusText,
              textAlign: TextAlign.center,
              style: const TextStyle(color: Colors.grey),
            ),
            const SizedBox(height: 24),
            LinearProgressIndicator(value: provider.downloadProgress / 100),
            const SizedBox(height: 4),
            Text(
              '${provider.downloadProgress.toStringAsFixed(1)}%',
              textAlign: TextAlign.right,
              style: const TextStyle(fontSize: 12, color: Colors.grey),
            ),
          ],
        );
    }
  }
}
