using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using DocumentProcessing;
using DocumentProcessing.Model;
using DocumentProcessing.GUI;

namespace DocumentProcessing
{
	/// <summary>
	/// Support class for changing coordinate systems
	/// </summary>
	public class DocumentViewerSupport
	{
		public static Rectangle GetImageSelectionToClientRectangle(AxMODI.AxMiDocView viewer,DocumentArea DocumentArea)
		{
			int x1,y1,x2,y2;
			viewer.ImageToClient(DocumentArea.Page,DocumentArea.Area.X,DocumentArea.Area.Y,out x1,out y1);
			viewer.ImageToClient(DocumentArea.Page,DocumentArea.Area.Right,DocumentArea.Area.Bottom,out x2,out y2);
			return new Rectangle(x1,y1,x2 - x1,y2 - y1);
		}
		
		public static DocumentArea GetScreenToImageRectangle(AxMODI.AxMiDocView viewer,Rectangle screenRectangle)
		{
			int viewPage = 0;
			DocumentArea context = new DocumentArea();
			
			Rectangle viewTarget = DocumentViewerSupport.GetScreenToImageRectangle(viewer,screenRectangle, out viewPage);
		
			context.Area = viewTarget;
			context.Page = viewPage;
			
			return context;
		}

		private static Rectangle GetScreenToImageRectangle(AxMODI.AxMiDocView viewer,Rectangle r, out int page)
		{
			Rectangle clientRect =r; 
			Point cp = viewer.PointToClient(new Point(r.X,r.Y));
			clientRect.X = cp.X;
			clientRect.Y = cp.Y;
			page = -1;
			int ppage1 , imgx1 ,imgy1 ;
			int ppage2 , imgx2 ,imgy2 ;
			try 
			{
				viewer.ClientToImage(clientRect.Left,clientRect.Top,out ppage1,out imgx1, out imgy1);
				viewer.ClientToImage(clientRect.Right,clientRect.Bottom,out ppage2,out imgx2, out imgy2);
			}
			catch( Exception exc)
			{
				throw new DocumentAreaException("Invalid bounds! Frame must be within a single page!");
			}
			if (ppage1 != ppage2) 
				throw new DocumentAreaException("Invalid bounds (2 pages selected)! Frame must be within a single page!");

			page = ppage1;
			Rectangle rect = new Rectangle(imgx1,imgy1, imgx2-imgx1,imgy2-imgy1); 
			return rect;
		}

		public static DocumentArea GetImageSelection(AxMODI.AxMiDocView viewer)
		{
			try 
			{
				int page,left,top,right,bottom;
				viewer.ImageSelection.GetBoundingRect(out page,out left, out top, out right, out bottom);
				DocumentArea DocumentArea = new DocumentArea();
				DocumentArea.Area = new Rectangle(left,top,right-left,bottom-top);
				DocumentArea.Page = page;
				return DocumentArea;
			}
			catch(Exception ee)
			{
				return null;
			}
		}

		public static Rectangle GetImageSelectionAreaToScreen(AxMODI.AxMiDocView viewer)
		{
			DocumentArea DocumentArea = GetImageSelection(viewer);
			if (DocumentArea == null) return Rectangle.Empty;
			Rectangle clientRect = GetImageSelectionToClientRectangle(viewer,DocumentArea);
			Rectangle screenRect = viewer.RectangleToScreen(clientRect);
			return screenRect;
		}
	}
	
}
