using System;
using System.Drawing;
using System.Collections;

using Microsoft.Ink;

using Featurefy;
using Fragmenter;
using Sketch;

namespace Toolkit
{
	/// <summary>
	/// Summary description for FragmentPanel.
	/// </summary>
	public class FragmentPanel : System.Windows.Forms.Panel
	{
		private Featurefy.FeatureStroke[] featureStrokes;
		
		private Microsoft.Ink.InkPicture sketchInk;

		private Microsoft.Ink.InkOverlay overlayInk;

		
		/// <summary>
		/// Previous scaling factor. Used in resizing the Ink and the Form.
		/// </summary>
		private float prevScale = 1.0f;
		
		/// <summary>
		/// New scaling factor. Used in resizing the Ink and the Form.
		/// </summary>
		private float newScale = 1.0f;
		
		/// <summary>
		/// Total scaling factor. Used in resizing the Ink and the Form.
		/// </summary>
		private float totalScale = 1.0f;

		
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="originalSketch"></param>
		public FragmentPanel(Sketch.Sketch originalSketch)
		{
			// Create the featured strokes
			Sketch.Stroke[] strokes = originalSketch.Strokes;
			featureStrokes = new Featurefy.FeatureStroke[strokes.Length];

			for (int i = 0; i < strokes.Length; i++)
			{
				featureStrokes[i] = new FeatureStroke(strokes[i]);
			}
			
			// Initialize the InkOverlay
			sketchInk = new InkPicture();
			sketchInk.EditingMode = InkOverlayEditingMode.Select;
			sketchInk.SelectionMoved += new InkOverlaySelectionMovedEventHandler(sketchInk_SelectionMoved);
			
			this.Resize += new EventHandler(FragmentPanel_Resize);
			
			//oInk.SelectionChanged += new InkOverlaySelectionChangedEventHandler(oInk_SelectionChanged);
			
			// Setup the Ink in the InkOverlay
			for (int i = 0; i < featureStrokes.Length; i++)
			{
				System.Drawing.Point[] pts = new System.Drawing.Point[featureStrokes[i].Points.Length];
				for (int k = 0; k < pts.Length; k++)
				{
					pts[k] = new System.Drawing.Point( (int)(featureStrokes[i].Points[k].X), 
						(int)(featureStrokes[i].Points[k].Y) );
				}
			
				sketchInk.Ink.CreateStroke(pts);
			}

			// Move center the ink's origin to the top-left corner
			sketchInk.Ink.Strokes.Move(-sketchInk.Ink.GetBoundingBox().X, -sketchInk.Ink.GetBoundingBox().Y);
			sketchInk.Enabled = true;

			// Give the panel the mInk component
			this.Controls.Add(sketchInk);

			// Update paint functionality
			this.Paint += new System.Windows.Forms.PaintEventHandler(FragmentPanel_Paint);

			
			// Another layer of ink overlaying the current
			// Used when drawing the fragment points
			overlayInk = new InkOverlay(sketchInk);

			// Initialize the overlay panel with the fragmented corners
			FragmentCorners(System.Drawing.Color.Red, 45);
		}



		private void FragmentPanel_Resize(object sender, System.EventArgs e)
		{
			InkResize();
		}

		private void FragmentPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// NOTE: probably don't want to do this unless we have to
			// Make it lazy
			//InkResize();
		}


		/// <summary>
		/// Creates the fragment corners found for the sketch
		/// </summary>
		/// <param name="color">Color of the corners</param>
		/// <param name="thickness">How thick we should draw the corners</param>
		private void FragmentCorners(Color color, int thickness) 
		{
			ArrayList ptsArray = new ArrayList();
			
			for (int i = 0; i < this.featureStrokes.Length; i++)
			{
				int[] corners = new Corners(this.featureStrokes[i]).FindCorners();
				Microsoft.Ink.Stroke stroke = sketchInk.Ink.Strokes[i];

				for (int k = 0; k < corners.Length; k++)
				{
					ptsArray.Add(stroke.GetPoint(corners[k]));
				}
			}
			
			System.Drawing.Point[] pts = (System.Drawing.Point[])ptsArray.ToArray(typeof(System.Drawing.Point));
			
			// Create strokes consisting of one point each
			// This is done so that we can draw the points in our InkPicture and correctly scale the points
			// accordingly.
			for (int i = 0; i < pts.Length; i++)
			{
				System.Drawing.Point[] p = new System.Drawing.Point[1];
				p[0] = pts[i];

				overlayInk.Ink.CreateStroke(p);
			}

			// Display features of the point (Color, Width, and Height)
			Microsoft.Ink.DrawingAttributes da = new Microsoft.Ink.DrawingAttributes(color);
			da.Height = thickness;
			da.Width  = thickness;

			// Render each point
			foreach (Microsoft.Ink.Stroke s in overlayInk.Ink.Strokes)
			{
				s.DrawingAttributes = da;
			}	
		}
				
		
		
		private void InkResize()
		{
			const double MARGIN = 0.03;

			// Actual stroke bounding box (in Ink Space)
			int strokeWidth  = sketchInk.Ink.Strokes.GetBoundingBox().Width;
			int strokeHeight = sketchInk.Ink.Strokes.GetBoundingBox().Height;
			
			int inkWidth  = this.Width - Convert.ToInt32(this.Width * MARGIN);
			int inkHeight = this.Height - Convert.ToInt32(this.Height * MARGIN);
		
			System.Drawing.Graphics g = this.CreateGraphics();

			if (strokeWidth != 0 && strokeHeight != 0)
			{
				// If we want to scale by the panel's width
				if (/*this.panelWidthBox.Checked ==*/ true)
				{
					// Convert the rendering space from Ink Space to Pixels
					System.Drawing.Point bottomRight = new System.Drawing.Point(strokeWidth, strokeHeight);
					sketchInk.Renderer.InkSpaceToPixel(g, ref bottomRight); 				
				
					System.Drawing.Point topLeft = new System.Drawing.Point(0, 0);
					sketchInk.Renderer.InkSpaceToPixel(g, ref topLeft); 				
				
					System.Drawing.Point scalePt = new System.Drawing.Point(bottomRight.X - topLeft.X, 
						bottomRight.Y - topLeft.Y);
				
					// Scale the rendered strokes by the width scaling factor
					float xScale = (float)inkWidth / (float)scalePt.X;
					float yScale = (float)inkHeight / (float)scalePt.Y;
		
					float scale = xScale;

					// Scale the stroke rendering by the scaling factor
					sketchInk.Renderer.Scale(scale, scale, false);

					// Scale the fragment point rendering by the scaling factor
					overlayInk.Renderer.Scale(scale, scale, true);

					// Resize the mInk component to encompass all of the scaled strokes
					System.Drawing.Rectangle scaledBoundingBox = sketchInk.Ink.GetBoundingBox();
					System.Drawing.Point scaledBottomRight = new System.Drawing.Point(scaledBoundingBox.Width,
						scaledBoundingBox.Height);

					sketchInk.Renderer.InkSpaceToPixel(g, ref scaledBottomRight); 

					sketchInk.Size = new Size(scaledBottomRight.X, scaledBottomRight.Y);
				
					// Update the scaling factors
					totalScale *= scale;
					prevScale = totalScale;

					// Update the user-displayed zoom
					//zoomTextBox.Text = totalScale.ToString();
				}
				else
				{
					if (prevScale != 0.0f)
						sketchInk.Renderer.Scale(newScale / prevScale, newScale / prevScale, false);
					
					this.totalScale = prevScale = newScale;	
				}
			}
		}



		/// Handles the InkOverlay to ensure that displayed strokes have not been moved.
		/// Code pulled from: http://windowssdk.msdn.microsoft.com/en-us/library/microsoft.ink.inkoverlay.selectionmoved.aspx
		/// </summary>
		/// <param name="sender">Reference to the object that raised the event</param>
		/// <param name="e">Selection moving event</param>
		private void sketchInk_SelectionMoved(object sender, InkOverlaySelectionMovedEventArgs e)
		{
			// Get the selection's bounding box
			System.Drawing.Rectangle newBounds = sketchInk.Selection.GetBoundingBox();

			// Move to back to original spot
			sketchInk.Selection.Move(e.OldSelectionBoundingRect.Left - newBounds.Left,
				e.OldSelectionBoundingRect.Top - newBounds.Top);

			// Trick to insure that selection handles are updated
			sketchInk.Selection = sketchInk.Selection;
		}
	}
}
