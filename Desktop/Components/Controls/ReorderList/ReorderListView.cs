using GongSolutions.Wpf.DragDrop;
using System;
using System.Windows;
using System.Windows.Media;

namespace StreamBoard.Components.Controls.ReorderList
{
    public class ReorderListView : Wpf.Ui.Controls.ListView, IDragSource
    {
        static ReorderListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ReorderListView),
                new FrameworkPropertyMetadata(typeof(Wpf.Ui.Controls.ListView)));
        }

        public ReorderListView()
        {
            GongSolutions.Wpf.DragDrop.DragDrop.SetIsDragSource(this, true);
            GongSolutions.Wpf.DragDrop.DragDrop.SetIsDropTarget(this, true);
            GongSolutions.Wpf.DragDrop.DragDrop.SetUseDefaultDragAdorner(this, true);
            GongSolutions.Wpf.DragDrop.DragDrop.SetDefaultDragAdornerOpacity(this, 0.7);

            GongSolutions.Wpf.DragDrop.DragDrop.SetDragHandler(this, this);

            SetResourceReference(StyleProperty, typeof(Wpf.Ui.Controls.ListView));
            SetResourceReference(ItemContainerStyleProperty, typeof(Wpf.Ui.Controls.ListViewItem));
        }


        public void StartDrag(IDragInfo dragInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.StartDrag(dragInfo);
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.VisualSourceItem is FrameworkElement item)
            {
                var wrapper = FindVisualChild<ReorderListItemWrapper>(item);

                if (wrapper != null && wrapper.UseDragHandle)
                {
                    var originalSource = System.Windows.Input.Mouse.DirectlyOver as DependencyObject;

                    var icon = FindVisualParent<StreamBoard.Components.Controls.FluentIcon>(originalSource);

                    if (icon == null)
                    {
                        return false;
                    }
                }
            }

            return GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.CanStartDrag(dragInfo);
        }

        public void Dropped(IDropInfo dropInfo) { }
        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo) { }
        public void DragCancelled() { }
        public bool TryCatchOccurredException(Exception exception) => false;

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent) return parent;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}