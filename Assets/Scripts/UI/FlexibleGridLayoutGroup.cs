using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RollingTundra.UI
{
    public class FlexibleGridLayoutGroup : GridLayoutGroup
    {
        public override void CalculateLayoutInputVertical ()
        {
            RecalculateSizes();

            base.CalculateLayoutInputVertical();
        }
        public override void CalculateLayoutInputHorizontal ()
        {
            RecalculateSizes();

            base.CalculateLayoutInputHorizontal();
        }
        protected override void OnRectTransformDimensionsChange ()
        {
            base.OnRectTransformDimensionsChange();

            RecalculateSizes();
        }

        protected virtual void RecalculateSizes ()
        {
            if (constraint == Constraint.FixedColumnCount)
            {
                float finalSize = rectTransform.rect.width - padding.left - padding.right - spacing.x * constraintCount;
                m_CellSize = new Vector2(finalSize / constraintCount, cellSize.y);
                SetDirty();
                return;
            }
            if (constraint == Constraint.FixedRowCount)
            {
                float finalSize = rectTransform.rect.height - padding.top - padding.bottom - spacing.y * constraintCount;
                m_CellSize = new Vector2(cellSize.x, finalSize / constraintCount);
                SetDirty();
                return;
            }
        }
    }
}