using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace DocumentProcessing.Model
{
	/// <summary>
	/// Geometric position within a document. 
	/// Positions and distances are in image coordinates.
	/// </summary>
	public class DocumentArea
	{
		private int _page;
		public int Page
		{
			get { return _page; }
			set { _page = value; }
		}

		private Rectangle _area; 
		public Rectangle Area
		{
			get { return _area; }
			set { _area = value; }
		}
	}
	
	public class DocumentAreaException : Exception
	{
		public DocumentAreaException(string description)
		{
			Description = description;
		}
		private string _Description; 
		public string Description
		{
			get { return _Description; }
			set { _Description= value; }
		}
	}

	public class Document
	{
		private ArrayList _pages = new ArrayList(); // of Page
		public ArrayList Pages
		{
			get { return _pages; }
			set { _pages = value;}
		}

		public Page Page
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
			}
		}

		public static Document CreateByMODI(MODI.Document document)
		{
			Document doc = new Document();
			for (int p = 0; p < document.Images.Count; p++)
			{
				Page page = new Page();
				doc.Pages.Add(page);
				MODI.Image image = (MODI.Image) document.Images[p];
				for (int i = 0; i < image.Layout.Words.Count; i++)
				{
					MODI.Word MODIWord = (MODI.Word) image.Layout.Words[i];
					Word newWord = Word.CreateBYMODI(MODIWord);

					page.Words.Add(newWord);
					
				}
			}
			return doc;
		}

		/// <summary>
		/// Clusters word elements to line elements in the selection area in the document.
		/// </summary>
		/// <param name="selection"></param>
		/// <returns></returns>
		public ArrayList GenerateLines( DocumentArea selection)
		{
			// iterate through all words and cluster to lines..
			ArrayList lines = new ArrayList();
			Page page =  this.Pages[ selection.Page] as Page;
			for (int i = 0; i < page.Words.Count; i++)
			{
				Word word = page.Words[i] as Word;

				// check, if word is already in existing line
				bool isElementInLine = false;
				for (int j = 0; j < lines.Count; j++)
				{
					Line line = (Line) lines[j];
					if (line.IsElementInLine(word.Bounds))
					{
						line.AddWord(word);
						isElementInLine = true;
						break;
					}
				}
				if (!isElementInLine)
				{
					// create new line
					Line line = new Line();
					lines.Add(line);
					line.AddWord(word);
				}
			}

			lines.Sort(new LineYSorter());
			return lines;
		}
	}
	
	
	public class Page
	{
		private ArrayList _words = new ArrayList(); // of Word
		public ArrayList Words
		{
			get { return _words; }
			set { _words = value;}
		}

		public Word Word
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
			}
		}
	}

	/// <summary>
	///  Line model
	/// </summary>
	public class Line
	{
		private ArrayList _words = new ArrayList(); // of Word
		public ArrayList Words
		{
			get { return _words; }
		}

		private bool _obsolete = true;

		public void AddWord(Word word)
		{
			_words.Add(word);
			_obsolete = true;
		}

		private Rectangle _bounds ;
		public Rectangle Bounds
		{
			get { return GetBounds(); }
		}
		
		private string _content ;
		public string Content 
		{
			get { return GetContent(); }
		}

		private Rectangle GetBounds()
		{
			if (_obsolete)
			{
				Update();
			}
			return _bounds;
		}
		private string GetContent()
		{
			if (_obsolete)
			{
				Update();
			}
			return _content;
		}

		private void Update()
		{
			Rectangle newBounds = Rectangle.Empty;
			string newContent = "";
			Words.Sort(new WordXSorter());
			for (int i = 0; i< Words.Count;i++)
			{
				Word word = (Word) Words[i];
				if (newBounds.IsEmpty) newBounds =  word.Bounds;
				newBounds = Rectangle.Union(newBounds,word.Bounds);
				newContent += word.Reference.Text + " ";
			}
			_bounds = newBounds ;
			_content = newContent.Trim();
			_obsolete = false;
		}

		/// <summary>
		/// The Clustering criteria to check if an element's (e.g. word's) is part of a line element.
		/// This is a very simple implementation of criteria. It only checks to intersection of the elements.
		/// More sophisticated implementations would use border line of the elements, font comparision, horizontal distances etc.
		/// </summary>
		/// <param name="elementBounds"></param>
		/// <returns></returns>
		public bool IsElementInLine(Rectangle elementBounds)
		{
			// expand line bounds to archive intersection
			Rectangle expandedLineBounds = Bounds;
			expandedLineBounds.X = elementBounds.Left;
			expandedLineBounds.Width = elementBounds.Right;
			return expandedLineBounds.IntersectsWith(elementBounds);

		}

		public Word Word
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
			}
		}
	}

	
	/// <summary>
	/// Word model
	/// </summary>
	public class Word
	{
		private MODI.Word _ref = null;
		public MODI.Word Reference
		{
			get { return _ref; }
			set { _ref = value; }
		}
		private Rectangle _bounds ;
		public Rectangle Bounds
		{
			get { return _bounds; }
			set { _bounds = value; }
		}
		/// <summary>
		/// Converts a MODI element to a Word instance
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		public static Word CreateBYMODI(MODI.Word word)
		{
			Word newWord = new Word();
			newWord.Reference = word;
			Rectangle bounds = Rectangle.Empty;
			for (int i = 0; i < word.Rects.Count; i++)
			{
				MODI.MiRect rect = (MODI.MiRect) word.Rects[i];
				Rectangle newRect = new Rectangle(rect.Left,		rect.Top,rect.Right-rect.Left,		rect.Bottom-rect.Top);
				if (bounds.IsEmpty) bounds = newRect;
				bounds = Rectangle.Union(bounds,newRect);
			}
			newWord.Bounds = bounds;
			return newWord;
		}
	}


	public class WordXSorter: IComparer
	{
		public int Compare(object x, object y)
		{
			Word x1 =  x as Word ;
			Word x2 = y as Word ;
			if (x1.Bounds.X == x2.Bounds.X) return 0;
			if (x1.Bounds.X > x2.Bounds.X) return 1;
			return -1;
		}
	}

	
	public class LineYSorter: IComparer
	{
		public int Compare(object x, object y)
		{
			Line x1 =  x as Line ;
			Line x2 = y as Line ;
			if (x1.Bounds.Y == x2.Bounds.Y) return 0;
			if (x1.Bounds.Y > x2.Bounds.Y) return 1;
			return -1;
		}
	}


	/// <summary>
	/// Column request model
	/// </summary>
	public class ColumnRequest
	{
		private float _width = 1f;
		/// <summary>
		/// Width relative to the table's width (1: full table width)
		/// </summary>
		public float Width
		{
			get { return _width ; }
			set { _width  = value; }
		}

	}
	
	/// <summary>
	/// Table request model
	/// </summary>
	public class TableRequest
	{
		private DocumentArea _area = null; 
		public DocumentArea DocumentArea
		{
			get { return _area; }
			set { _area = value; }
		}
		
		private ArrayList _ColumnRequests = new ArrayList();
		public ArrayList ColumnRequests
		{
			get { return _ColumnRequests; }
			set { _ColumnRequests = value; }
		}

	
		/// <summary>
		/// Evaluates the Columns Bounds refering to the current request area.
		/// As long as we dont use caching, we have to calculate the x offset first.
		/// </summary>
		/// <returns></returns>
		private Rectangle GetColumnBounds(int index)
		{	
			if (index > _ColumnRequests.Count) throw new Exception("Wrong parameter");
			float left  = GetColumnOffset(index);
			float width = (_ColumnRequests[index] as ColumnRequest).Width;
			return 
				new Rectangle(
				this.DocumentArea.Area.Left+ ((int)( this.DocumentArea.Area.Width*left)), 
				this.DocumentArea.Area.Top,
				((int)(this.DocumentArea.Area.Width*width)),
				this.DocumentArea.Area.Height);

		}

		/// <summary>
		/// Evaluates the Columns X Positon refering to the current request area.
		/// As long as we dont use caching, we have to calculate it every time.
		/// </summary>
		/// <returns></returns>
		private float GetColumnOffset(int index)
		{
			if (index > _ColumnRequests.Count) throw new Exception("Wrong parameter");
			float offset = 0;
			for (int i = 0; i < index; i++)
			{
				ColumnRequest colRequest = (ColumnRequest) _ColumnRequests[i];
				offset += colRequest.Width;
			}
			return offset;

		}

		/// <summary>
		/// evaluate, which column area contains the given element
		/// </summary>
		/// <param name="elementBounds"></param>
		/// <returns></returns>
		private ColumnRequest GetContainingColumn(Rectangle elementBounds)
		{
			for (int i = 0; i < _ColumnRequests.Count; i++)
			{
				Rectangle columnBounds = GetColumnBounds(i);
				if (columnBounds.IntersectsWith( elementBounds))
					return  (ColumnRequest) _ColumnRequests[i];;
			}
			return null;
		}
		
		
		/// <summary>
		/// This is the main method for extraction
		/// </summary>
		/// <param name="document"></param>
		/// <returns></returns>
		public TableResult GetTableContent(Document document)
		{
			TableResult result = new TableResult(this);
			
			// generate lines..
			ArrayList lines = document.GenerateLines( DocumentArea);

			result.Init(lines);
			
			for (int i =0; i < lines.Count; i++)
			{
				Line line = (Line) lines[i];
				string s = line.Content;
				// iterate through all contained words and decide for each one, where it belongs to..
				for (int j = 0; j < line.Words.Count; j++)
				{
					Word word = (Word) line.Words[j];
					ColumnRequest column = GetContainingColumn(word.Bounds);
					if (column != null)
					{
						ColumnResult columnResult = result.GetColumnResult(line,column);
						if (columnResult != null)
						{
							// add word to the corresponding result element
							columnResult.Words.Add(word);
						}
					}
				}
				
			}
			return result;
		}

	}
	

	/// <summary>
	/// Table result model
	/// </summary>
	public class TableResult
	{
		private ArrayList _lineResults = new ArrayList(); // of LineResult
		public ArrayList LineResults
		{
			get { return _lineResults ; }
			set { _lineResults  = value; }
		}

		/// <summary>
		/// Corresponding request
		/// </summary>
		public TableRequest TableRequest
		{
			get { return _TableRequest ; }
			set { _TableRequest  = value; }
		}
			
		private TableRequest _TableRequest = null;
		public TableResult(TableRequest request)
		{
			_TableRequest = request;
		}

		public ColumnResult GetColumnResult(Line line,ColumnRequest column)
		{
			for (int i = 0; i < _lineResults.Count; i++)
			{
				LineResult lineResult = (LineResult) _lineResults[i];
				if (lineResult.LineReference == line)
				{
					return lineResult.GetColumnResult(column);
				}
			}
			return null;
		}

		
		public void FillListViewColumns(System.Windows.Forms.ListView listview)
		{
			listview.Columns.Clear();
			for (int i = 0; i < TableRequest.ColumnRequests.Count; i++)
			{
				listview.Columns.Add(i.ToString(),50,HorizontalAlignment.Left);
			}
		}

		/// <summary>
		/// Fill content to listview. 
		/// </summary>
		/// <param name="listview"></param>
		public void FillListViewItems(System.Windows.Forms.ListView listview)
		{
			listview.Items.Clear();
			for (int i = 0; i < _lineResults.Count; i++)
			{
				LineResult lineResult = (LineResult) _lineResults[i];
				ListViewItem newItem = new ListViewItem();
				for (int j = 0; j  < lineResult.ColumnResults.Count; j++)
				{
					ColumnResult colResult = (ColumnResult)  lineResult.ColumnResults[j];
					if (j == 0)
					{
						newItem.Text = colResult.GetContent();
					}
					else
						newItem.SubItems.Add(colResult.GetContent());
				}
				listview.Items.Add(newItem);
			}

		}

		
		public void Init(ArrayList lines)
		{
			for (int i = 0; i < lines.Count; i++)
			{
				Line line = (Line) lines[i] as Line;
				if (TableRequest.DocumentArea.Area.IntersectsWith( line.Bounds))
				{
					LineResult newLine = new LineResult();
					newLine.LineReference =	line;
					for (int j = 0; j < TableRequest.ColumnRequests.Count; j++)
					{
						ColumnResult newColumn = new ColumnResult();
						newColumn.ColumnRequest = TableRequest.ColumnRequests[j] as ColumnRequest;
						newLine.ColumnResults.Add(newColumn);
					}
					this.LineResults.Add(newLine);
				}
			}
		}


		public void ExportToFile(string filename, string separator)
		{
			StreamWriter writer =  File.CreateText(filename);
			for (int i = 0; i < _lineResults.Count; i++)
			{
				LineResult lineResult = (LineResult) _lineResults[i];
				string lineContent = "";
					for (int j = 0; j  < lineResult.ColumnResults.Count; j++)
				{
					ColumnResult colResult = (ColumnResult)  lineResult.ColumnResults[j];
					string colContent = colResult.GetContent();

					lineContent += colContent ;
					if(j  < lineResult.ColumnResults.Count-1) lineContent += separator;
				}
				writer.WriteLine(lineContent);
			}
			writer.Close();
		}
	}


	/// <summary>
	/// Line result model
	/// </summary>
	public class LineResult
	{
		private ArrayList _ColumnResults = new ArrayList(); // of ColumnResult
		public ArrayList ColumnResults
		{
			get { return  _ColumnResults ; }
			set { _ColumnResults = value; }
		}


		public Line LineReference
		{
			get { return  _LineRef ; }
			set {_LineRef = value; }
		}
		
		private Line _LineRef = null;	
		public ColumnResult GetColumnResult(ColumnRequest column)
		{
			for (int i = 0; i < _ColumnResults.Count; i++)
			{
				ColumnResult result = (ColumnResult) _ColumnResults[i];
				if (result.ColumnRequest == column)
				{
					return result;
				}
			}
			return null;
		}
	}


	/// <summary>
	/// Column result model
	/// </summary>
	public class ColumnResult
	{
		private ArrayList _Words = new ArrayList(); // of Word
		public ArrayList Words
		{
			get { return  _Words ; }
			set { _Words = value; }
		}


		/// <summary>
		/// Corresponding line request
		/// </summary>
		public ColumnRequest ColumnRequest
		{
			get { return _ColumnRequest ; }
			set { _ColumnRequest = value; }
		}
 
		private ColumnRequest _ColumnRequest = null; 
		
		/// <summary>
		/// Simple string serialization
		/// </summary>
		/// <returns></returns>
		public string GetContent()
		{
			// without caching.. therefore pretty slow!
			string content = "";
			for (int i =0; i< Words.Count; i++)
			{
				Word word = (Word) Words[i];
				content += word.Reference.Text + " ";
			}
			return content.Trim();
		}
	}


}
