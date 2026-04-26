using UglyToad.PdfPig;
using UglyToad.PdfPig.Outline;
using UglyToad.PdfPig.Outline.Destinations;
using UglyToad.PdfPig.Writer;
namespace UsePDFSharpFaultPattern;

public class UnitTest_PdfPig
{
	/// <summary>
	/// 画像だけのPDFをインポートした場合のテスト
	/// </summary>
	/// <returns></returns>
	[Fact]
	public void Test01_ImportImagePage()
	{
		ImportPdfAndSave( "img005.pdf", "PdfPig_Test01_ImportImagePage.pdf" );
	}

	/// <summary>
	/// S-JIS 保存されている日本語名の非埋め込みフォントを持つPDFをインポートした場合のテスト
	/// </summary>
	/// <returns></returns>
	[Fact]
	public void Test02_ImportShiftJisPage()
	{
		ImportPdfAndSave( "Sample32.pdf", "PdfPig_Test02_ImportShiftJisPage.pdf" );
	}

	private static void ImportPdfAndSave( string sourceFileName, string outputFileName )
	{
		string baseDirectory = AppContext.BaseDirectory;
		string inputPath = Path.Combine(baseDirectory, "TestData", sourceFileName);
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, outputFileName);

		Directory.CreateDirectory( outputDirectory );
		// PdfPig を使って、inputPath のPDFを読み込む
		using var inputDocument = PdfDocument.Open(inputPath);
		// PdfPig は PdfDocumentBuilder で新規ドキュメントを作成する
		var outputDocument = new PdfDocumentBuilder();
		// リンク情報も無条件コピーとする
		// PdfPigはページ番号が1ベースであることに注意
		for (int i = 1; i <= inputDocument.NumberOfPages; i++)
		{
			outputDocument.AddPage( inputDocument, i );
		}
		// outputDocument を保存する(バイト配列になるのはなんでなのかね？Streamに直接保存してくれればいいのにねぇ…
		var bytes = outputDocument.Build();
		File.WriteAllBytes( outputPath, bytes );
	}
	/// <summary>
	/// S-JIS 保存されている日本語名の非埋め込みフォントを持つPDF に、ブックマークをつけた場合のテスト
	/// </summary>
	/// <returns></returns>
	[Fact]
	public void Test03_AddFilenameBookmark()
	{
		string baseDirectory = AppContext.BaseDirectory;
		string inputPath = Path.Combine(baseDirectory, "TestData", "Sample32.pdf");
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, "PdfPig_Test03_AddFilenameBookmark.pdf");

		Directory.CreateDirectory( outputDirectory );
		
		// PdfPig を使って、inputPath のPDFを読み込む
		using var inputDocument = PdfDocument.Open(inputPath);
		// PdfPig は PdfDocumentBuilder で新規ドキュメントを作成する
		var outputDocument = new PdfDocumentBuilder();
		
		// ページをコピー
		for (int i = 1; i <= inputDocument.NumberOfPages; i++)
		{
			outputDocument.AddPage( inputDocument, i );
		}
		var bookamrkExplict = new ExplicitDestination( 1, ExplicitDestinationType.FitPage, ExplicitDestinationCoordinates.Empty );
		var bookmarkNode = new DocumentBookmarkNode( "Sample32.pdf", 0, bookamrkExplict, Array.Empty<BookmarkNode>() );
		outputDocument.Bookmarks = new Bookmarks( new List<BookmarkNode> { bookmarkNode } );
		// outputDocument を保存する
		var bytes = outputDocument.Build();
		File.WriteAllBytes( outputPath, bytes );
	}
	/// <summary>
	/// 閲覧パスワード付きのPDFにしおりを追加した場合の動作確認
	/// </summary>
	/// <returns></returns>
	[Fact(Skip = "PdfPig 0.1.14 does not support adding bookmarks via PdfDocumentBuilder. Bookmarks class is read-only.")]
	public async Task Test04_AddBookmarkWithPassword1()
	{
		string baseDirectory = AppContext.BaseDirectory;
		string inputPath = Path.Combine(baseDirectory, "TestData", "1_閲覧PW付き_PW=0000.pdf");
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, "PdfPig_Test04_AddBookmarkWithPassword.pdf");

		Directory.CreateDirectory( outputDirectory );
		throw new NotImplementedException();
	}
	/// <summary>
	/// 編集パスワード付きPDFにしおりを追加した場合の動作確認
	/// </summary>
	/// <returns></returns>
	[Fact(Skip = "PdfPig 0.1.14 does not support adding bookmarks via PdfDocumentBuilder. Bookmarks class is read-only.")]
	public async Task Test05_AddBookmarkWithPassword2()
	{
		string baseDirectory = AppContext.BaseDirectory;
		string inputPath = Path.Combine(baseDirectory, "TestData", "2_編集PW付き_PW=0000.pdf");
		string outputDirectory = Path.Combine(baseDirectory, "Output");
		string outputPath = Path.Combine(outputDirectory, "PdfPig_Test05_AddBookmarkWithPassword.pdf");

		Directory.CreateDirectory( outputDirectory );
		throw new NotImplementedException();
	}
	[Fact]
	public void Test06_MergePdfFile()
	{
	}
}