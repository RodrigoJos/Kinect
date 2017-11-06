using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor miKinect;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MostrarVideo_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("Não há kinect conectado.");
                Application.Current.Shutdown();
            }         
            try
            {
                miKinect = KinectSensor.KinectSensors[0];                
                miKinect.SkeletonStream.Enable();
                miKinect.ColorStream.Enable();
                miKinect.Start();

                
                    miKinect.ColorFrameReady += miKinect_ColorFrameReady;

                    miKinect.SkeletonFrameReady += miKinect_SkeletonFrameReady;
                
            }
            catch
            {
                MessageBox.Show("Erro ao iniciar o kinect.");
                Application.Current.Shutdown();
            }

        }
            void miKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
            {
                using (ColorImageFrame frameImage = e.OpenColorImageFrame())
                {
                    if (frameImage == null) return;

                    byte[] dadosColor = new byte[frameImage.PixelDataLength];

                    frameImage.CopyPixelDataTo(dadosColor);

                    MostrarVideo.Source = BitmapSource.Create(frameImage.Width, frameImage.Height, 96, 96, PixelFormats.Bgr32, null, dadosColor, frameImage.Width * frameImage.BytesPerPixel);
                }
            }
            
        void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {            
            Skeleton[] esqueletos = null;

            using (SkeletonFrame framesEsqueleto = e.OpenSkeletonFrame())
            {
                if(framesEsqueleto != null)
                {
                    esqueletos = new Skeleton[framesEsqueleto.SkeletonArrayLength];
                    framesEsqueleto.CopySkeletonDataTo(esqueletos);
                }
            }

            if (esqueletos == null) return;

            foreach (Skeleton esqueleto in esqueletos)
            {
                if(esqueleto.TrackingState == SkeletonTrackingState.Tracked)
                {
                   
                    //Verifica se o esqueleto está posicinado errado.
                    if (esqueleto.ClippedEdges != 0)
                    {
                        //Verifica se o esqueleto está muito pra baixo.
                        if ((esqueleto.ClippedEdges & FrameEdges.Bottom) != 0)
                        {
                            tbStatus.Text = string.Format("Move-se para cima!!!");
                        }
                        //Verifica se o esqueleto está muito pra cima
                        if ((esqueleto.ClippedEdges & FrameEdges.Top) != 0)
                        {
                            tbStatus.Text = string.Format("Move-se para baixo");
                        }
                        //Verifica se o esqueleto está muito pra direita
                        if ((esqueleto.ClippedEdges & FrameEdges.Right) != 0)
                        {
                            tbStatus.Text = string.Format("Move-se para a esquerda.");
                        }
                        //Verifica se o esqueleto está muito pra esquerda.
                        if ((esqueleto.ClippedEdges & FrameEdges.Left) != 0)
                        {
                            tbStatus.Text = string.Format("Move-se para a direita.");
                        }
                    }
                    else
                    {
                        tbStatus.Text = string.Format("Posição perfeita.");
                        //Está em uma posição perfeita, então pode pegar as coordenadas do corpo.
                        Joint jointCabeca = esqueleto.Joints[JointType.Head];
                        Joint jointMaoDireita = esqueleto.Joints[JointType.HandRight];
                        Joint jointOmbro = esqueleto.Joints[JointType.ShoulderCenter];
                        Joint jointQuadril = esqueleto.Joints[JointType.HipCenter];
                        Joint jointJoelho = esqueleto.Joints[JointType.KneeRight];
                        Joint jointPe = esqueleto.Joints[JointType.FootRight];

                        SkeletonPoint posicaoCabeca = jointCabeca.Position;
                        SkeletonPoint posicaoMaoDireita = jointMaoDireita.Position;
                        SkeletonPoint posicaoOmbro = jointOmbro.Position;
                        SkeletonPoint posicaoQuadril = jointQuadril.Position;
                        SkeletonPoint posicaoJoelho = jointJoelho.Position;
                        SkeletonPoint posicaoPe = jointPe.Position;

                        
                        

                        tbMsg.Text = string.Format(" Joelho:{0:0.0}, Ombro:{1:0.0}, Cabeça:{2:0.0}, Quadril:{3:0.0}", posicaoJoelho.Y,posicaoOmbro.Y,posicaoCabeca.Y,posicaoQuadril.Y);
                        
                        if( posicaoMaoDireita.Y >= posicaoOmbro.Y)
                        {
                            tbStatus.Text = string.Format("Mao levantada.");
                        }
                        else
                        {
                            tbStatus.Text = string.Format("Mao abaixada.");
                        }
                            
                        
                        
                    }
                }                
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void tbStatus_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
