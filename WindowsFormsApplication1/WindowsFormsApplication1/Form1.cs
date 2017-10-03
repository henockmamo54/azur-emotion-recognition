using System.Windows.Forms;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Drawing;
using WindowsFormsApplication1.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        OpenFileDialog openFileDialog1;
        RootObject resoponseImageEmotion;
        List<RootObject> multipleImageEmotionResponse;
        float scalefactor_w = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            openFileDialog1 = new OpenFileDialog();
            multipleImageEmotionResponse = new List<RootObject>();
        }

        byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        async void MakeRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "99126fd28ac54031bc0f0331c706225f");

            // NOTE: You must use the same region in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westcentralus, replace "westus" in the 
            //   URI below with "westcentralus".
            string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";
            HttpResponseMessage response;
            string responseContent;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                responseContent = response.Content.ReadAsStringAsync().Result;
                

                multipleImageEmotionResponse = JsonConvert.DeserializeObject<List<RootObject>>(responseContent);
                
                showResponse();
                pictureBox1.Refresh();

            }

            //A peak at the JSON response.
            Console.WriteLine(responseContent);
        }

        public void showResponse()
        {

            txt_anger.Text  =  txt_contempt.Text = txt_disgust.Text = txt_fear.Text = txt_happiness.Text =
                txt_neutral.Text = txt_sadness.Text = txt_surprise.Text = " " ;

            foreach (RootObject response in multipleImageEmotionResponse)
            {

                txt_anger.Text += ": " + Math.Round(response.scores.anger * 100, 2);
                txt_contempt.Text += ": " + Math.Round(response.scores.contempt * 100, 2);
                txt_disgust.Text += ": " + Math.Round(response.scores.disgust * 100, 2);
                txt_fear.Text += ": " + Math.Round(response.scores.fear * 100, 2);
                txt_happiness.Text += ": " + Math.Round(response.scores.happiness * 100, 2);
                txt_neutral.Text += ": " + Math.Round(response.scores.neutral * 100, 2);
                txt_sadness.Text += ": " + Math.Round(response.scores.sadness * 100, 2);
                txt_surprise.Text += ": " + Math.Round(response.scores.surprise * 100, 2);
            }


            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                multipleImageEmotionResponse = null;
                Console.WriteLine(openFileDialog1.FileName);

                string imageFilePath = openFileDialog1.FileName;
                imageEmotion_filepath.Text = imageFilePath;
                MakeRequest(imageFilePath);

                pictureBox1.Image = Image.FromFile(imageFilePath);
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;



            if (multipleImageEmotionResponse != null)
            {

                int index = 1;
                // Create font and brush.
                Font drawFont = new Font("Arial", 16);
                SolidBrush drawBrush = new SolidBrush(Color.Green);                

                foreach (RootObject resoponseImageEmotion in multipleImageEmotionResponse)
                {

                    if (resoponseImageEmotion != null)
                    {


                        scalefactor_w = pictureBox1.Image.Width / (float)pictureBox1.Width;
                        if (scalefactor_w < 1)
                        {
                            scalefactor_w = pictureBox1.Width / (float)pictureBox1.Image.Width;
                            float scalefactor_h = pictureBox1.Height / (float)pictureBox1.Image.Height;

                            Rectangle rect = new Rectangle((int)(resoponseImageEmotion.faceRectangle.left * scalefactor_w), (int)(resoponseImageEmotion.faceRectangle.top * scalefactor_h),
                            (int)(resoponseImageEmotion.faceRectangle.width * scalefactor_w), (int)(resoponseImageEmotion.faceRectangle.height * scalefactor_h));

                            // Create point for upper-left corner of drawing.
                            PointF drawPoint = new PointF((int)(resoponseImageEmotion.faceRectangle.left * scalefactor_w), (int)(resoponseImageEmotion.faceRectangle.top * scalefactor_h));

                            g.DrawRectangle(System.Drawing.Pens.Red, rect);
                            g.DrawString(index + "", drawFont, drawBrush, drawPoint);
                        }
                        else
                        {

                            float scalefactor_h = pictureBox1.Image.Height / (float)pictureBox1.Height;

                            Rectangle rect = new Rectangle((int)(resoponseImageEmotion.faceRectangle.left / scalefactor_w), (int)(resoponseImageEmotion.faceRectangle.top / scalefactor_h),
                            (int)(resoponseImageEmotion.faceRectangle.width / scalefactor_w), (int)(resoponseImageEmotion.faceRectangle.height / scalefactor_h));

                            PointF drawPoint = new PointF((int)(resoponseImageEmotion.faceRectangle.left / scalefactor_w), (int)(resoponseImageEmotion.faceRectangle.top / scalefactor_h));

                            g.DrawRectangle(System.Drawing.Pens.Red, rect);
                            g.DrawString(index + "", drawFont, drawBrush, drawPoint);
                        }
                    }

                    index++;
                }


            }
        }
    }
}
