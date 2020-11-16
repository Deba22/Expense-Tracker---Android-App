using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace ExpenseTracker.Helper
{
    public class MarginItemDecoration_Hor : RecyclerView.ItemDecoration
    {
        private readonly bool _addSpaceFirstItem;
        private readonly bool _addSpaceLastItem;
        private readonly int _space;

        public MarginItemDecoration_Hor(int space, bool addSpaceFirstItem, bool addSpaceLastItem)
        {
            _space = space;
            _addSpaceFirstItem = addSpaceFirstItem;
            _addSpaceLastItem = addSpaceLastItem;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);
            if (_space <= 0) return;
            if (_addSpaceFirstItem && parent.GetChildLayoutPosition(view) < 1 ||
                parent.GetChildLayoutPosition(view) >= 1)
            {
                if (getOrientation(parent) == LinearLayoutManager.Vertical)
                {
                    outRect.Top = _space;
                }
                else
                {
                    outRect.Left = _space;
                }
            }

            if (!_addSpaceLastItem || parent.GetChildAdapterPosition(view) != getTotalItemCount(parent) - 1) return;
            if (getOrientation(parent) == LinearLayoutManager.Vertical)
            {
                outRect.Bottom = _space;
            }
            else
            {
                outRect.Right = _space;
            }
        }

        private int getOrientation(RecyclerView parent)
        {
            if (parent.GetLayoutManager() is LinearLayoutManager)
            {
                return ((LinearLayoutManager)parent.GetLayoutManager()).Orientation;
            }
            throw new NotSupportedException("SpaceItemDecoration can only be used with a LinearLayoutManager.");
        }

        private int getTotalItemCount(RecyclerView parent)
        {
            return parent.GetAdapter().ItemCount;
        }
    }
}
