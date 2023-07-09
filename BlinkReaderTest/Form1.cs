using BlinkReader.Forms;
using BlinkReader;

using PokemonBDSPRNGLibrary.StarterRNG;
using PokemonPRNG.XorShift128;

namespace BlinkReaderTest
{
    public partial class Form1 : Form
    {
        private const double FPS = 60.0;
        CaptureWindowForm captureWindowForm = new CaptureWindowForm();
        CancellationTokenSource cts = new CancellationTokenSource();
        public Form1()
        {
            InitializeComponent();
            captureWindowForm.Show();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            MunchlaxInverter munchlaxInverter = new MunchlaxInverter();
            (uint, uint, uint, uint) restored = default;

            //string srcFilePath = "E:\\pictures\\eye.png";
            string srcFilePath = "E:\\pictures\\gombe.png";
            var detector = new BlinkDetector(srcFilePath);

            var en = BlinkCapturerer.CaptureBlinkAsync(cts.Token, detector, captureWindowForm, FPS).GetAsyncEnumerator();

            long prev = 0;
            long current = 0;
            while (await en.MoveNextAsync())
            {
                current = (long)en.Current;
                float elapsed = (float)((current - prev)/ 10_000_000.0);
                if (prev != 0)
                {
                    munchlaxInverter.AddInterval(elapsed);
                    if (munchlaxInverter.TryRestoreState(out restored)) break;
                    this.label1.Text = elapsed.ToString();
                }
                prev = current;
            }

            if (restored == default) return;


            restored.Advance((uint)munchlaxInverter.BlinkCount);
            var nextInterval = restored.BlinkMunchlax();
            var countdown = nextInterval - (DateTime.Now.Ticks - current)/10_000_000.0;

            var pt = OriginalTimer.PeridoricTimerAsync(cts.Token, FPS).GetAsyncEnumerator();
            while (await pt.MoveNextAsync())
            {
                this.label1.Text = countdown.ToString("F2");
                countdown -= 1.0 / FPS;
                if (countdown <= 0) countdown = restored.BlinkMunchlax();
                
            }
            detector.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
        }
    }
}