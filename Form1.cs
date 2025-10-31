using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Face;

namespace OpenCV_Yuzalgilama
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private VideoCapture _capture;
        private CascadeClassifier _cascadeClassifier;

        private void Form1_Load(object sender, EventArgs e)
        {
            // Yüz algılama için cascade dosyasını yükleyin
            // Dosyanın adını doğru yazdığınızdan emin olun
            _cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            // 0 varsayılan kameradır. Farklı bir kamera için 1, 2 vb. deneyebilirsiniz.
            _capture = new VideoCapture(0);

            // Her kare yakalandığında 'ProcessFrame' metodunu çalıştır.
            Application.Idle += ProcessFrame;

            button_start.Enabled = false; // Butonu devre dışı bırak.

        }
        private void ProcessFrame(object sender, EventArgs e)
        {
            if (_capture == null || !_capture.IsOpened) return;

            // Kameradan bir kare yakala (Bu, Mat tipinde bir nesnedir)
            using (var matFrame = _capture.QueryFrame())
            {
                if (matFrame == null) return;

                // *** YENİ: Mat nesnesini Image<Bgr, byte> tipine dönüştürerek çizim yapılabilir hale getiriyoruz. ***
                using (var imageFrame = matFrame.ToImage<Bgr, byte>())
                {
                    if (imageFrame == null) return;

                    // Görüntüyü gri tonlamaya dönüştür (algılama için)
                    var grayFrame = imageFrame.Convert<Gray, byte>();

                    // Yüzleri algıla
                    Rectangle[] faces = _cascadeClassifier.DetectMultiScale(
                        grayFrame,
                        1.1,     // Scale Factor
                        10,      // Minimum Komşular
                        new Size(20, 20) // Minimum Yüz Boyutu
                    );

                    // Algılanan her yüz için çerçeve çiz (Artık imageFrame.Draw metodunu kullanabiliriz)
                    foreach (var face in faces)
                    {
                        // Çizimi Image<TColor, TDepth> nesnesi üzerinde yapıyoruz
                        imageFrame.Draw(face, new Bgr(Color.Red), 3); // Kırmızı çerçeve çiz
                    }

                    // İşlenmiş görüntüyü PictureBox'ta göster
                    // Image<TColor, TDepth> nesnesini Bitmap'e dönüştürerek gösteriyoruz
                    camera.Image = imageFrame.ToBitmap();
                }
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Idle -= ProcessFrame; // Olayı durdur
            if (_capture != null)
            {
                _capture.Dispose(); // Kaynakları serbest bırak
            }
            if (_cascadeClassifier != null)
            {
                _cascadeClassifier.Dispose();
            }
        }

        private void camera_Click(object sender, EventArgs e)
        {

        }
    }
}
