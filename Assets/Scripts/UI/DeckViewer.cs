using UnityEngine;
using UnityEngine.UI;

public class DeckViewer : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;
    public FitType fitType;
    public bool fitX;
    public bool fitY;

    public float width;
    public float height;
    
    int numOfRowsOnScreen = 3;

    new void OnEnable() {
    
        base.OnEnable();
        
        this.rectTransform.sizeDelta = new Vector2(width, height);

        int amount = rows - numOfRowsOnScreen;
        amount = (amount > 0) ? amount : 0;
        this.rectTransform.sizeDelta = new Vector2(width , height + (375) * amount); 
    }
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if(fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            fitX = true;
            fitY = true;

            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }

        if(fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float) columns);
        }
        if(fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float) rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / (float)columns - ((spacing.x / (float) columns) * (columns - 1)) - (padding.left / (float) columns) - (padding.right) / (float)columns;
        float cellHeight = parentHeight / (float)rows - ((spacing.y / (float) rows) * (rows - 1)) - (padding.top / (float) rows) - (padding.bottom / (float) rows);

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for(int i=0;i<rectChildren.Count;i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            float preferredWidth = 0;
            float preferredHeight = 0;
            if(item.TryGetComponent<LayoutElement>(out LayoutElement layoutElement))
            {
                preferredWidth = layoutElement.preferredWidth;
                preferredHeight = layoutElement.preferredHeight;
            }
            if(preferredWidth > 0)
            {
                cellSize.x = preferredWidth;
            }
            if(preferredHeight > 0)
            {
                cellSize.y = preferredHeight;
            }
            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }


    }
    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}