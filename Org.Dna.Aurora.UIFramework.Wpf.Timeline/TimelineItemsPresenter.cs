using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline
{

    public class TimelineItemsPresenter : ItemsControl
    {

        static TimelineItemsPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineItemsPresenter), new FrameworkPropertyMetadata(typeof(TimelineItemsPresenter)));

            ItemsPanelTemplate defaultPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(TimelineCompactPanel)));

            ItemsPanelProperty.OverrideMetadata(typeof(TimelineItemsPresenter), new FrameworkPropertyMetadata(defaultPanel));

            ItemContainerStyleSelectorProperty.AddOwner(typeof(TimelineItemsPresenter), new FrameworkPropertyMetadata(null, CoerceItemContainerStyleSelector));
        }

        private static object CoerceItemContainerStyleSelector(DependencyObject d, object baseValue)
        {
            object value = baseValue ?? new StyleSelectorByItemType();
            return value;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is TimelineItem);
        }

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new TimelineItem();
        }

        internal TimelineItem ContainerFromItem(object item)
        {
            return base.ItemContainerGenerator.ContainerFromItem(item) as TimelineItem;
        }
    }
}
