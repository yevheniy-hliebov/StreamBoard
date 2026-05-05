import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:streamtabula/common/widgets/orientation_adaptive_scaffold.dart';
import 'package:streamtabula/core/constants/app_colors.dart';
import 'package:streamtabula/core/constants/spacing.dart';
import 'package:streamtabula/features/settings/providers/settings_provider.dart';
import 'package:streamtabula/features/updater/presentations/update_dialog.dart';
import 'package:streamtabula/features/updater/providers/updater_provider.dart';
import 'package:streamtabula/features/updater/services/app_info_service.dart';

class SettingsScreen extends StatelessWidget {
  const SettingsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final settings = context.watch<SettingsProvider>();
    final appInfo = context.read<AppInfoService>().appInfo;

    return OrientationAdaptiveScaffold(
      title: 'Settings',
      leading: IconButton(
        icon: const Icon(Icons.arrow_back),
        onPressed: () => Navigator.pop(context),
      ),
      body: ListView(
        children: [
          _buildSectionHeader(context, 'Server'),
          ListTile(
            leading: const Icon(Icons.dns),
            title: const Text('Server Connection'),
            subtitle: Text(
              settings.isConfigured
                  ? '${settings.address}:${settings.port}'
                  : 'Not configured',
            ),
            onTap: () => _showServerDialog(context, settings),
          ),

          const Divider(),

          _buildSectionHeader(context, 'Updates'),
          SwitchListTile(
            secondary: const Icon(Icons.bug_report),
            title: const Text('Beta Releases'),
            subtitle: const Text(
              'Receive early access updates (may be unstable)',
            ),
            value: settings.receiveBetaUpdates,
            onChanged: (bool value) {
              context.read<SettingsProvider>().toggleBetaUpdates(value);
            },
          ),
          ListTile(
            leading: const Icon(Icons.system_update),
            title: const Text('Check for Updates'),
            subtitle: const Text('Manually check for new versions'),
            onTap: () {
              final updaterProvider = context.read<UpdaterProvider>();
              final appInfo = context.read<AppInfoService>().appInfo;
              final receiveBeta = context
                  .read<SettingsProvider>()
                  .receiveBetaUpdates;
              updaterProvider.performManualUpdateCheck(appInfo, receiveBeta);

              showDialog(
                context: context,
                barrierDismissible: false,
                builder: (context) => const UpdateDialog(),
              );
            },
          ),

          const SizedBox(height: Spacing.xxl),

          Center(
            child: Text(
              '${appInfo.appName} v${appInfo.currentVersion}',
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                color: AppColors.of(context).onDisabled,
              ),
            ),
          ),
          const SizedBox(height: Spacing.md),
        ],
      ),
    );
  }

  Widget _buildSectionHeader(BuildContext context, String title) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        Spacing.md,
        Spacing.md,
        Spacing.md,
        Spacing.sm,
      ),
      child: Text(
        title,
        style: Theme.of(
          context,
        ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold),
      ),
    );
  }

  void _showServerDialog(BuildContext context, SettingsProvider settings) {
    showDialog(
      context: context,
      builder: (context) => _ServerConnectionDialog(
        initialAddress: settings.address,
        initialPort: settings.port,
      ),
    );
  }
}

class _ServerConnectionDialog extends StatefulWidget {
  final String initialAddress;
  final String initialPort;

  const _ServerConnectionDialog({
    required this.initialAddress,
    required this.initialPort,
  });

  @override
  State<_ServerConnectionDialog> createState() =>
      _ServerConnectionDialogState();
}

class _ServerConnectionDialogState extends State<_ServerConnectionDialog> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _addressController;
  late TextEditingController _portController;

  @override
  void initState() {
    super.initState();
    _addressController = TextEditingController(text: widget.initialAddress);
    _portController = TextEditingController(text: widget.initialPort);
  }

  @override
  void dispose() {
    _addressController.dispose();
    _portController.dispose();
    super.dispose();
  }

  void _save() {
    if (_formKey.currentState!.validate()) {
      context.read<SettingsProvider>().saveServerSettings(
        _addressController.text.trim(),
        _portController.text.trim(),
      );
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Settings saved & connected')),
      );
      Navigator.pop(context);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Server Connection'),
      content: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextFormField(
              controller: _addressController,
              decoration: InputDecoration(
                labelText: 'IP Address / Hostname',
                hintStyle: TextStyle(color: AppColors.of(context).onPrimary),
                labelStyle: TextStyle(color: AppColors.of(context).onPrimary),
                border: const OutlineInputBorder(),
                hintText: 'e.g., 192.168.1.2 or localhost',
              ),
              validator: (value) =>
                  value == null || value.isEmpty ? 'Cannot be empty' : null,
            ),
            const SizedBox(height: Spacing.md),
            TextFormField(
              controller: _portController,
              decoration: InputDecoration(
                labelText: 'Port',
                hintStyle: TextStyle(color: AppColors.of(context).onPrimary),
                labelStyle: TextStyle(color: AppColors.of(context).onPrimary),
                border: const OutlineInputBorder(),
                hintText: 'e.g., 13550',
              ),
              keyboardType: TextInputType.number,
              validator: (value) =>
                  value == null || value.isEmpty ? 'Cannot be empty' : null,
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Cancel'),
        ),
        FilledButton(onPressed: _save, child: const Text('Save & Connect')),
      ],
    );
  }
}
