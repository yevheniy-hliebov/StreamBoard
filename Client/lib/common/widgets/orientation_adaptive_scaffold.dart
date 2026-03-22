import 'package:flutter/material.dart';
import 'package:streamboard/core/constants/spacing.dart';

class OrientationAdaptiveScaffold extends StatelessWidget {
  final Widget body;
  final Widget? leading;
  final String title;
  final List<Widget>? actions;

  const OrientationAdaptiveScaffold({
    super.key,
    required this.body,
    this.leading,
    this.title = '',
    this.actions,
  });

  @override
  Widget build(BuildContext context) {
    return OrientationBuilder(
      builder: (context, orientation) {
        if (orientation == Orientation.portrait) {
          return _buildPortraitLayout(context);
        } else {
          return _buildLandscapeLayout(context, isLeft: true);
        }
      },
    );
  }

  Widget _buildPortraitLayout(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: leading,
        centerTitle: true,
        title: Text(title, style: Theme.of(context).textTheme.titleSmall),
        actions: actions,
      ),
      body: body,
    );
  }

  Widget _buildLandscapeLayout(BuildContext context, {required bool isLeft}) {
    final sideBar = _buildSideBar(context, isLeft: isLeft);

    return Scaffold(
      body: SafeArea(
        child: Row(
          children: [
            if (isLeft) sideBar,
            Expanded(child: body),
            if (!isLeft) sideBar,
          ],
        ),
      ),
    );
  }

  Widget _buildSideBar(BuildContext context, {required bool isLeft}) {
    return Container(
      width: 72,
      color: Theme.of(context).colorScheme.surface,
      padding: const EdgeInsets.symmetric(vertical: Spacing.md),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          if (isLeft) _buildLeading() else _buildActions(),

          Expanded(
            child: Center(
              child: RotatedBox(
                quarterTurns: isLeft ? 3 : 1,
                child: Text(
                  title,
                  style: Theme.of(context).textTheme.titleSmall,
                  textAlign: TextAlign.center,
                ),
              ),
            ),
          ),

          if (isLeft) _buildActions() else _buildLeading(),
        ],
      ),
    );
  }

  Widget _buildLeading() {
    if (actions != null) {
      return Column(
        spacing: Spacing.xs,
        mainAxisSize: MainAxisSize.min,
        children: actions!,
      );
    } else {
      return const SizedBox(height: 48);
    }
  }

  Widget _buildActions() {
    if (leading != null) {
      return leading!;
    } else {
      return const SizedBox(height: 48);
    }
  }
}
