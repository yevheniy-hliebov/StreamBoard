import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:streamboard/features/updater/presentations/update_dialog.dart';
import 'package:streamboard/features/updater/providers/updater_provider.dart';
import 'package:streamboard/features/updater/services/app_info_service.dart';
import 'package:wakelock_plus/wakelock_plus.dart';
import 'package:streamboard/common/widgets/orientation_adaptive_scaffold.dart';
import 'package:streamboard/core/constants/app_colors.dart';
import 'package:streamboard/core/constants/spacing.dart';
import 'package:streamboard/features/home/presentaions/widgets/grid_deck_layout.dart';
import 'package:streamboard/features/home/presentaions/widgets/setup_connection_prompt.dart';
import 'package:streamboard/features/home/providers/deck_provider.dart';
import 'package:streamboard/features/settings/presentations/screens/settings_screen.dart';
import 'package:streamboard/features/settings/providers/settings_provider.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  @override
  void initState() {
    super.initState();
    WakelockPlus.enable();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _runAutoUpdateCheck();
    });
  }

  @override
  void dispose() {
    WakelockPlus.disable();
    super.dispose();
  }

  Future<void> _runAutoUpdateCheck() async {
    final updaterProvider = context.read<UpdaterProvider>();
    final settings = context.read<SettingsProvider>();
    final appInfo = context.read<AppInfoService>().appInfo;

    final shouldShowDialog = await updaterProvider.checkForUpdatesOnStartup(
      appInfo,
      settings,
    );

    if (shouldShowDialog && mounted) {
      showDialog(
        context: context,
        barrierDismissible: false,
        builder: (context) => const UpdateDialog(),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final isConfigured = context.watch<SettingsProvider>().isConfigured;

    return OrientationAdaptiveScaffold(
      title: context.watch<DeckProvider>().currentPageName.isEmpty
          ? 'StreamBoard'
          : context.watch<DeckProvider>().currentPageName,
      leading: IconButton(
        icon: Icon(Icons.refresh, color: AppColors.of(context).onBackground),
        onPressed: isConfigured
            ? () => context.read<DeckProvider>().fetchDeck()
            : null,
      ),
      actions: [
        IconButton(
          icon: Icon(Icons.settings, color: AppColors.of(context).onBackground),
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
