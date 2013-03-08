//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace WindowsStoreCppDemo;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace ZXing::QrCode;
using namespace ZXing;
using namespace ZXing::Common;
using namespace ZXing::QrCode;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage::Streams ;
using namespace Windows::Graphics::Imaging;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

MainPage::MainPage()
{
	InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	(void) e;	// Unused parameter
}


void WindowsStoreCppDemo::MainPage::btnName_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	
	String^ strName =txtInputText->Text;
	if (strName->IsEmpty())
	{
		auto dialog = ref new Windows::UI::Popups::MessageDialog("Plz Enter Some Text ");
		dialog-> ShowAsync();
		txtInputText->Focus(Windows::UI::Xaml::FocusState::Pointer);	
		return;
	}
	BarcodeWriter^ barcodeWriter = ref new BarcodeWriter();
	barcodeWriter->Format = BarcodeFormat::QR_CODE;
	this->lastBitmap = barcodeWriter->Write(strName);
	imgPlaceHolder->Source = lastBitmap;
}


void WindowsStoreCppDemo::MainPage::btnDecode_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	BarcodeReader^ barcodeReader = ref new BarcodeReader();
   Result^ result = barcodeReader->Decode(this->lastBitmap);
   if (result != nullptr)
   {
      txtDecodedText->Text = result->Text;
   }
}
