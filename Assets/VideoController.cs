/* 
이 스크립트는 Video를 관리하기 위해 만들어진 스크립트로, 재생/종료 등의 기능을 수행할 수 있다.
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
        //비디오 로딩이 종료되었는가?
        public bool isVideoPrepared = false;
        //비디오 음소거인가?
        public bool isVideoMuted = false;
        //비디오가 실행으로 설정되어 있는가?
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
        
        //영상을 토대로 스크롤을 설정하는 함수
        void SetScroll()
        {
            if (isVideoPrepared && videoPlayer.isPlaying && !videoPlayer.isPaused)
            {
                scroll.value = (float)(videoPlayer.time / videoPlayer.length);
            }
        }

        //스크롤을 누를 시 비디오 정지 및 클릭 위치로 포인터 이동
        public void PointerDownOnScroll()
        {
            PauseVideo();
            float videoPercentage = Input.mousePosition.x / scroll.GetComponent<RectTransform>().rect.width;
            scroll.value = videoPercentage;
        }

        //스크롤을 뗄 시, 그 비율에 맞춰 비디오 이동
        public void PointerUpOnScroll()
        {
            float videoPercentage = scroll.value;
            videoPlayer.time = videoPlayer.length * videoPercentage;
            SetScroll();
            if(isVideoSettedByPlaying)
                PlayVideo();
        }

        //초를 시간으로 변환
        public string SecToTime(double time)
        {
            int hour, minute, second;

            //시간공식
            hour = (int)(time / 3600);//시 공식
            minute = (int)(time % 3600 / 60);//분을 구하기위해서 입력되고 남은값에서 또 60을 나눈다.
            second = (int)(time % 3600 % 60);//마지막 남은 시간에서 분을 뺀 나머지 시간을 초로 계산함.
            return hour.ToString() + ":" + minute.ToString() + ":" + second.ToString();
        }

        //비디오 연동
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

        //정지
        public void PlayVideo()
        {
            videoPlayer.Play();
        }

        //시작
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

        //초기화
        public void ResetVideo()
        {
            videoPlayer.Stop();
            videoPlayer.Play();
        }

        //음소거
        public void MuteVideo()
        {
            isVideoMuted = !isVideoMuted;
            videoPlayer.SetDirectAudioMute(0, isVideoMuted);
        }

        //종료시 비디오 준비 끝
        private void OnDestroy()
        {
            videoPlayer.prepareCompleted -= VideoPlayerPreparedCompleted;
        }
    }
}