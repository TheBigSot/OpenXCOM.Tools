using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MapView.Forms.MapObservers.RmpViews;
using XCom;
using XCom.Interfaces.Base;

namespace MapView.Forms.MapObservers.TopViews
{
	public class SimpleMapPanel : Map_Observer_Control
	{
		//private DSShared.Windows.RegistryInfo registryInfo;

		private int _offX = 0;
	    private int _offY = 0; 
	    protected int MinimunHeight = 4;

		private readonly GraphicsPath _cell;
		private readonly GraphicsPath _copyArea;
		private readonly GraphicsPath _selected;
	    private Point _sel1;
	    private Point _sel2;
	    private Point _sel3;
	    private Point _sel4;

	    private int _mR;
	    private int _mC;
	    protected DrawContentService DrawContentService = new DrawContentService() ;

	    public SimpleMapPanel()
		{
			_cell = new GraphicsPath();
			_selected = new GraphicsPath();
			_copyArea = new GraphicsPath();

			_sel1 = new Point(0, 0);
			_sel2 = new Point(0, 0);
			_sel3 = new Point(0, 0);
			_sel4 = new Point(0, 0);
		}

	    public void ParentSize(int width, int height)
		{
			if (map != null)
			{
                int oldWid = DrawContentService.HWidth;
			    var hWidth = DrawContentService.HWidth;
                var hHeight = DrawContentService.HHeight;
			    if (map.MapSize.Rows > 0 || map.MapSize.Cols > 0)
			    {
			        if (height > width / 2)
			        {
			            //use width
                        hWidth = width / (map.MapSize.Rows + map.MapSize.Cols);

			            if (hWidth % 2 != 0)
			                hWidth--;

			            hHeight = hWidth / 2;
			        }
			        else
			        {
			            //use height
			            hHeight = height / (map.MapSize.Rows + map.MapSize.Cols);
			            hWidth = hHeight * 2;
			        }
			    }

			    if (hHeight < MinimunHeight)
				{
					hWidth = MinimunHeight * 2;
					hHeight = MinimunHeight;
				}

			    DrawContentService.HWidth = hWidth;
			    DrawContentService.HHeight = hHeight;

				_offX = 4 + map.MapSize.Rows * hWidth;
				_offY = 4;

				if (oldWid != hWidth)
				{
					Width = 8 + (map.MapSize.Rows + map.MapSize.Cols) * hWidth;
					Height = 8 + (map.MapSize.Rows + map.MapSize.Cols) * hHeight;
					Refresh();
				}
			}
		}

		[Browsable(false)]
		[DefaultValue(null)]
		public override IMap_Base Map
		{
			set
			{
                map = value;
                DrawContentService.HWidth = 7;
				ParentSize(Parent.Width, Parent.Height);
				Refresh();
			}
		}

        protected void ViewDrag(object sender, MouseEventArgs ex)
		{
            var s = GetDragStart();
            var e = GetDragEnd();

            //                 col hei
            var hWidth = DrawContentService.HWidth;
            var hHeight = DrawContentService.HHeight;
            _sel1.X = _offX + (s.X - s.Y) * hWidth;
			_sel1.Y = _offY + (s.X + s.Y) * hHeight;

			_sel2.X = _offX + (e.X - s.Y) * hWidth + hWidth;
			_sel2.Y = _offY + (e.X + s.Y) * hHeight + hHeight;

			_sel3.X = _offX + (e.X - e.Y) * hWidth;
			_sel3.Y = _offY + (e.X + e.Y) * hHeight + hHeight + hHeight;

			_sel4.X = _offX + (s.X - e.Y) * hWidth - hWidth;
			_sel4.Y = _offY + (s.X + e.Y) * hHeight + hHeight;

			_copyArea.Reset();
			_copyArea.AddLine(_sel1, _sel2);
			_copyArea.AddLine(_sel2, _sel3);
			_copyArea.AddLine(_sel3, _sel4);
			_copyArea.CloseFigure();

			Refresh();
		}

	    private static Point GetDragEnd()
	    {
	        var e = new Point(0, 0);
	        e.X = Math.Max(MapViewPanel.Instance.View.DragStart.X, MapViewPanel.Instance.View.DragEnd.X);
	        e.Y = Math.Max(MapViewPanel.Instance.View.DragStart.Y, MapViewPanel.Instance.View.DragEnd.Y);
	        return e;
	    }

	    private static Point GetDragStart()
	    {
	        var s = new Point(0, 0);
	        s.X = Math.Min(MapViewPanel.Instance.View.DragStart.X, MapViewPanel.Instance.View.DragEnd.X);
	        s.Y = Math.Min(MapViewPanel.Instance.View.DragStart.Y, MapViewPanel.Instance.View.DragEnd.Y);
	        return s;
	    }

	    [Browsable(false), DefaultValue(null)]
	    public Dictionary<string, SolidBrush> Brushes { get; set; }

	    [Browsable(false), DefaultValue(null)]
	    public Dictionary<string, Pen> Pens { get; set; }

	    public override void SelectedTileChanged(IMap_Base sender, SelectedTileChangedEventArgs e)
		{
			MapLocation pt = e.MapPosition;

			Text = "r: " + pt.Row + " c: " + pt.Col;

            var hWidth = DrawContentService.HWidth;
            var hHeight = DrawContentService.HHeight;
			int xc = (pt.Col - pt.Row) * hWidth;
			int yc = (pt.Col + pt.Row) * hHeight;

			_selected.Reset();
			_selected.AddLine(xc, yc, xc + hWidth, yc + hHeight);
			_selected.AddLine(xc + hWidth, yc + hHeight, xc, yc + 2 * hHeight);
			_selected.AddLine(xc, yc + 2 * hHeight, xc - hWidth, yc + hHeight);
			_selected.CloseFigure();

			ViewDrag(null, null);
			Refresh();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (e.Delta > 0)
				map.Up();
			else
				map.Down();
		}

		private void convertCoordsDiamond(int x, int y, out int row, out int col)
		{
			//int x = xp - offX; //16 is half the width of the diamond
			//int y = yp - offY; //24 is the distance from the top of the diamond to the very top of the image

            var hWidth = DrawContentService.HWidth;
            var hHeight = DrawContentService.HHeight;
			double x1 = (x * 1.0 / (2 * hWidth)) + (y * 1.0 / (2 * hHeight));
			double x2 = -(x * 1.0 - 2 * y * 1.0) / (2 * hWidth);

			row = (int)Math.Floor(x2);
			col = (int)Math.Floor(x1);
			//return new Point((int)Math.Floor(x1), (int)Math.Floor(x2));
		}

		protected virtual void RenderCell(MapTileBase tile, Graphics g, int x, int y) { }


	    protected GraphicsPath CellPath(int xc, int yc)
        {
            var hWidth = DrawContentService.HWidth;
            var hHeight = DrawContentService.HHeight ;
			_cell.Reset();
			_cell.AddLine(xc, yc, xc + hWidth, yc + hHeight);
			_cell.AddLine(xc + hWidth, yc + hHeight, xc, yc + 2 * hHeight);
			_cell.AddLine(xc, yc + 2 * hHeight, xc - hWidth, yc + hHeight);
			_cell.CloseFigure();
			return _cell;
		}

		protected override void Render(Graphics g)
		{
			g.FillRectangle(SystemBrushes.Control, ClientRectangle);

            var hWidth = DrawContentService.HWidth;
            var hHeight = DrawContentService.HHeight;
			if (map != null)
			{
				for (int row = 0, startX = _offX, startY = _offY; row < map.MapSize.Rows; row++, startX -= hWidth, startY += hHeight)
				{
					for (int col = 0, x = startX, y = startY; col < map.MapSize.Cols; col++, x += hWidth, y += hHeight)
					{
						MapTileBase mapTile = map[row, col];

						if (mapTile != null)
							RenderCell(mapTile, g, x, y);
					}
				}

				for (int i = 0; i <= map.MapSize.Rows; i++)
					g.DrawLine(Pens["GridColor"], _offX - i * hWidth, _offY + i * hHeight, ((map.MapSize.Cols - i) * hWidth) + _offX, ((i + map.MapSize.Cols) * hHeight) + _offY);
				for (int i = 0; i <= map.MapSize.Cols; i++)
					g.DrawLine(Pens["GridColor"], _offX + i * hWidth, _offY + i * hHeight, (i * hWidth) - map.MapSize.Rows * hWidth + _offX, (i * hHeight) + map.MapSize.Rows * hHeight + _offY);

				if (_copyArea != null)
					g.DrawPath(Pens["SelectColor"], _copyArea);

				//				if(selected!=null) //clicked on
				//					g.DrawPath(new Pen(Brushes.Blue,2),selected);

				if (_mR < map.MapSize.Rows && _mC < map.MapSize.Cols && _mR >= 0 && _mC >= 0)
				{
					int xc = (_mC - _mR) * hWidth + _offX;
					int yc = (_mC + _mR) * hHeight + _offY;

					GraphicsPath selPath = CellPath(xc, yc);
					g.DrawPath(Pens["MouseColor"], selPath);
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			int row, col;
		    if (map == null) return;
			convertCoordsDiamond(e.X - _offX, e.Y - _offY,out row, out col);
			map.SelectedTile = new MapLocation(row,col, map.CurrentHeight);
			mDown = true;

			Point p = new Point(col, row);
		    ;
			MapViewPanel.Instance.View.DragStart = p;
			MapViewPanel.Instance.View.DragEnd = p;
		}

		private bool mDown = false;
		protected override void OnMouseUp(MouseEventArgs e)
		{
			mDown = false;
			MapViewPanel.Instance.View.Refresh();
			Refresh();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			int row, col;
			convertCoordsDiamond(e.X - _offX, e.Y - _offY,out row, out col);
			if (row != _mR || col != _mC)
			{
				_mR = row;
				_mC = col;

				if (mDown)
				{
					MapViewPanel.Instance.View.DragEnd = new Point(col,row);
					MapViewPanel.Instance.View.Refresh();
				    if (e.Button == MouseButtons.Left)
				    {
				        ViewDrag(null, e);
				    }
				}
				Refresh();
			}
		}
	}
}