using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace UsePDFSharpFaultPattern;

public class UnitTest_PdfSharp
{
	/// <summary>
	/// 画像だけのPDFをインポートした場合のテスト(問題なくインポートできている)
	/// </summary>
	/// <returns></returns>
	[Fact]
	public async Task Test01_ImportImagePageAsync()
	{
		await ImportPdfAndSaveAsync( "img005.pdf", "PdfSharp_Test01_ImportImagePage.pdf" );
	}

	/// <summary>
	/// S-JIS 保存されている日本語名の非埋め込みフォントを持つPDFをインポートした場合のテスト(意図したフォントが当たらなくなる)
	/// </summary>
	/// <returns></returns>
	[Fact]
	public async Task Test02_ImportShiftJisPageAsync()
	{
		await ImportPdfAndSaveAsync( "Sample32.pdf", "PdfSharp_Test02_ImportShiftJisPage.pdf" );
	}

	private static async Task ImportPdfAndSaveAsync(string sourceFileName, string outputFileName)
	{
		string baseDirectory = AppContext.BaseDirectory;
		string inputPath = Path.Combine(baseDirectory, "TestData", sourceFileName);
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, outputFileName);

		Directory.CreateDirectory(outputDirectory);

		using PdfDocument inputDocument = PdfReader.Open(inputPath, PdfDocumentOpenMode.Import);
		using PdfDocument outputDocument = new();

		for (int pageIndex = 0; pageIndex < inputDocument.PageCount; pageIndex++)
		{
			outputDocument.AddPage(inputDocument.Pages[pageIndex]);
		}

		await outputDocument.SaveAsync( outputPath );
	}

	/// <summary>
	/// S-JIS 保存されている日本語名の非埋め込みフォントを持つPDF に、ブックマークをつけた場合のテスト(ブックマークはつくが、意図したフォントが当たらなくなる)
	/// </summary>
	/// <returns></returns>
	[Fact]
	public async Task Test03_AddFilenameBookmarkAsync()
	{
		string baseDirectory = AppContext.BaseDirectory;
		string inputPath = Path.Combine(baseDirectory, "TestData", "Sample32.pdf");
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, "PdfSharp_Test03_AddFilenameBookmark.pdf");

		Directory.CreateDirectory( outputDirectory );
		// PDFを開いて、ブックマークを追加して保存
		using var document = PdfReader.Open(inputPath, PdfDocumentOpenMode.Modify);
		// 最初のページにブックマークを追加する
		document.Outlines.Add("Sample32.pdf", document.Pages[0]);
		await document.SaveAsync( outputPath );
	}
	/// <summary>
	/// 閲覧パスワード付きのPDFにしおりを追加した場合の動作確認(パスワードが消える)
	/// </summary>
	/// <returns></returns>
	[Fact]
	public async Task Test04_AddBookmarkWithPassword1()
	{
		string baseDirectory = AppContext.BaseDirectory;
		string inputPath = Path.Combine(baseDirectory, "TestData", "1_閲覧PW付き_PW=0000.pdf");
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, "PdfSharp_Test04_AddBookmarkWithPassword.pdf");

		Directory.CreateDirectory( outputDirectory );
		// PDFを開いて、ブックマークを追加して保存
		using var document = PdfReader.Open(inputPath, PdfDocumentOpenMode.Modify, ( arg ) =>
		{
			arg.Password = "0000";
			arg.Abort = false;
		} );
		// 最初のページにブックマークを追加する
		document.Outlines.Add("1_閲覧PW付き_PW=0000.pdf", document.Pages[0]);
		await document.SaveAsync( outputPath );
	}
	/// <summary>
	/// 編集パスワード付きPDFにしおりを追加した場合の動作確認(パスワードが消える)
	/// </summary>
	/// <returns></returns>
	[Fact]
	public async Task Test05_AddBookmarkWithPassword2()
	{
		string baseDirectory = AppContext.BaseDirectory;
		string inputPath = Path.Combine(baseDirectory, "TestData", "2_編集PW付き_PW=0000.pdf");
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, "PdfSharp_Test05_AddBookmarkWithPassword.pdf");

		Directory.CreateDirectory( outputDirectory );
		// PDFを開いて、ブックマークを追加して保存
		using var document = PdfReader.Open(inputPath, PdfDocumentOpenMode.Modify, ( arg ) =>
		{
			arg.Password = "0000";
			arg.Abort = false;
		} );
		// 最初のページにブックマークを追加する
		document.Outlines.Add( "2_編集PW付き_PW=0000.pdf", document.Pages[0] );
		await document.SaveAsync( outputPath );
	}
	[Fact]
	public async Task Test06_MergePdfFile()
	{
		// img005.pdf に、tede-msbuild.pdf をマージする
		// しおりもマージされるかのテスト
		string baseDirectory = AppContext.BaseDirectory;
		var srcFile1 = Path.Combine(baseDirectory, "TestData", "Sample32.pdf");
		var srcFile2 = Path.Combine(baseDirectory, "TestData", "tede-msbuild.pdf");
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, "PdfSharp_Test06_MergePdfFile.pdf");

		// 出力ディレクトリの作成
		Directory.CreateDirectory(outputDirectory);

		using PdfDocument inputDocument1 = PdfReader.Open(srcFile1, PdfDocumentOpenMode.Import);
		using PdfDocument inputDocument2 = PdfReader.Open(srcFile2, PdfDocumentOpenMode.Import);
		using PdfDocument outputDocument = new();

		MergeDocumentWithOutlines(inputDocument1, outputDocument);
		MergeDocumentWithOutlines(inputDocument2, outputDocument);

		await outputDocument.SaveAsync(outputPath);
	}
	[Fact]
	public async Task Test07_MergePdfFileWithFilenameBookmark()
	{
		// Test06の処理に追加して、ファイル名のブックマークをつける。
		// 既存ファイルにブックマークがある場合は、ファイル名のブックマークのサブアイテムとして追加する
		string baseDirectory = AppContext.BaseDirectory;
		var srcFile1 = Path.Combine(baseDirectory, "TestData", "Sample32.pdf");
		var srcFile2 = Path.Combine(baseDirectory, "TestData", "tede-msbuild.pdf");
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, "PdfSharp_Test07_MergePdfFileWithFilenameBookmark.pdf");

		// 出力ディレクトリの作成
		Directory.CreateDirectory( outputDirectory );

		using PdfDocument inputDocument1 = PdfReader.Open(srcFile1, PdfDocumentOpenMode.Import);
		using PdfDocument inputDocument2 = PdfReader.Open(srcFile2, PdfDocumentOpenMode.Import);
		using PdfDocument outputDocument = new();

		MergeDocumentWithOutlines( inputDocument1, outputDocument, Path.GetFileName(srcFile1) );
		MergeDocumentWithOutlines( inputDocument2, outputDocument, Path.GetFileName(srcFile2) );

		await outputDocument.SaveAsync( outputPath );
	}

	private static void MergeDocumentWithOutlines(PdfDocument sourceDocument, PdfDocument outputDocument, string? filename = null )
	{
		Dictionary<PdfPage, PdfPage> pageMap = [];
		PdfPage? dstPage = null;
		for (int pageIndex = 0; pageIndex < sourceDocument.PageCount; pageIndex++)
		{
			PdfPage sourcePage = sourceDocument.Pages[pageIndex];
			PdfPage addedPage = outputDocument.AddPage(sourcePage);
			pageMap[sourcePage] = addedPage;
			if( pageIndex == 0 )
			{
				dstPage = addedPage;
			}
		}
		PdfOutline? parentOutline = null;
		if( filename != null )
		{
			// ファイル名ブックマークを作成する。
			parentOutline = new PdfOutline()
			{
				Title = filename,
				Opened = true,
				DestinationPage = dstPage!,
			};
			outputDocument.Outlines.Add( parentOutline );
		}
		CopyOutlines( sourceDocument.Outlines, outputDocument.Outlines, pageMap, parentOutline );
	}

	private static void CopyOutlines(PdfOutlineCollection sourceOutlines, PdfOutlineCollection destinationOutlines, IReadOnlyDictionary<PdfPage, PdfPage> pageMap, PdfOutline? parentOutline )
	{
		PdfOutlineCollection targetOutlines = parentOutline?.Outlines ?? destinationOutlines;

		foreach (PdfOutline sourceOutline in sourceOutlines)
		{
			PdfPage? destinationPage = null;
			if (sourceOutline.DestinationPage is not null)
			{
				pageMap.TryGetValue(sourceOutline.DestinationPage, out destinationPage);
			}

			PdfOutline copiedOutline = new()
			{
				Title = sourceOutline.Title,
				Opened = sourceOutline.Opened,
				Style = sourceOutline.Style,
				TextColor = sourceOutline.TextColor,
				PageDestinationType = sourceOutline.PageDestinationType,
				Left = sourceOutline.Left,
				Top = sourceOutline.Top,
				Right = sourceOutline.Right,
				Bottom = sourceOutline.Bottom,
				Zoom = sourceOutline.Zoom,
				DestinationPage = destinationPage!,
			};

			targetOutlines.Add(copiedOutline);

			if (sourceOutline.Outlines.Count > 0)
			{
				// サブアイテムは変わらない
				CopyOutlines(sourceOutline.Outlines, copiedOutline.Outlines, pageMap, null );
			}
		}
	}
}
