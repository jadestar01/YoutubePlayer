/* 
�� ��ũ��Ʈ�� Video�� �����ϱ� ���� ������� ��ũ��Ʈ��, ���/���� ���� ����� ������ �� �ִ�.
2023.04.07
*/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace YoutubePlayer
{
    public class VideoController : MonoBehaviour
    {
        //URL
        public string URL;
        //���� �ε��� ����Ǿ��°�?
        public bool isVideoPrepared = false;
        //���� ���Ұ��ΰ�?
        public bool isVideoMuted = false;
        //������ �������� �����Ǿ� �ִ°�?
        public bool isVideoSettedByPlaying = false;
        public YoutubePlayer youtubePlayer;
        public VideoPlayer videoPlayer;
        public Button playButton;
        public Button pauseButton;
        public Button resetButton;
        public Button muteButton;
        public Scrollbar scroll;
        public TextMeshProUGUI time;

        private void Awake()
        {
            //
            youtubePlayer.youtubeUrl = URL;
            //
            playButton.interactable = false;
            pauseButton.interactable = false;
            resetButton.interactable = false;
            muteButton.interactable = false;

            videoPlayer.prepareCompleted += VideoPlayerPreparedCompleted; 
        }

        private void Update()
        {
            SetScroll();
            time.text = (SecToTime(videoPlayer.time) + "/" + SecToTime(videoPlayer.length));
        }

        void VideoPlayerPreparedCompleted(VideoPlayer source)
        {
            isVideoPrepared = true;
            playButton.interactable = source.isPrepared;
            pauseButton.interactable = source.isPrepared;
            resetButton.interactable = source.isPrepared;
            muteButton.interactable = source.isPrepared;
        }
        
        //������ ���� ��ũ���� �����ϴ� �Լ�
        void SetScroll()
        {
            if (isVideoPrepared && videoPlayer.isPlaying && !videoPlayer.isPaused)
            {
                scroll.value = (float)(videoPlayer.time / videoPlayer.length);
            }
        }

        //��ũ���� ���� �� ���� ���� �� Ŭ�� ��ġ�� ������ �̵�
        public void PointerDownOnScroll()
        {
            PauseVideo();
            float videoPercentage = Input.mousePosition.x / scroll.GetComponent<RectTransform>().rect.width;
            scroll.value = videoPercentage;
        }

        //��ũ���� �� ��, �� ������ ���� ���� �̵�
        public void PointerUpOnScroll()
        {
            float videoPercentage = scroll.value;
            videoPlayer.time = videoPlayer.length * videoPercentage;
            SetScroll();
            if(isVideoSettedByPlaying)
                PlayVideo();
        }

        //�ʸ� �ð����� ��ȯ
        public string SecToTime(double time)
        {
            int hour, minute, second;

            //�ð�����
            hour = (int)(time / 3600);//�� ����
            minute = (int)(time % 3600 / 60);//���� ���ϱ����ؼ� �Էµǰ� ���������� �� 60�� ������.
            second = (int)(time % 3600 % 60);//������ ���� �ð����� ���� �� ������ �ð��� �ʷ� �����.
            return hour.ToString() + ":" + minute.ToString() + ":" + second.ToString();
        }

        //���� ����
        public async void Prepare()
        {
            print("Loading Video...");
            try
            {
                await youtubePlayer.PrepareVideoAsync();
                print("Video Loading");
            }
            catch
            {
                print("ERROR : Cannot Load Video");
            }
        }

        //����
        public void PlayVideo()
        {
            videoPlayer.Play();
        }

        //����
        public void PauseVideo()
        {
            videoPlayer.Pause();
        }

        public void PlayAndPause()
        {
            isVideoSettedByPlaying = !isVideoSettedByPlaying;

            if (isVideoSettedByPlaying)
            {
                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Pause();
            }
        }

        //�ʱ�ȭ
        public void ResetVideo()
        {
            videoPlayer.Stop();
            videoPlayer.Play();
        }

        //���Ұ�
        public void MuteVideo()
        {
            isVideoMuted = !isVideoMuted;
            videoPlayer.SetDirectAudioMute(0, isVideoMuted);
        }

        //����� ���� �غ� ��
        private void OnDestroy()
        {
            videoPlayer.prepareCompleted -= VideoPlayerPreparedCompleted;
        }
    }
}