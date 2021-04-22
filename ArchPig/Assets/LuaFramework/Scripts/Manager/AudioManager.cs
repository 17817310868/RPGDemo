using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AudioManager
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
                instance = new AudioManager();
            return instance;
        }
    }

    Dictionary<string, AudioClip> audioClipsDic = new Dictionary<string, AudioClip>();

    ResourceManager ResMgr;

    private AudioSource bgmSource;
    private AudioSource effectSource;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="camera"></param>
    public void Init(GameObject camera)
    {
        ResMgr = AppFacade.Instance.GetManager<ResourceManager>(ManagerName.Resource);
        bgmSource = camera.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0.5f;
        effectSource = camera.AddComponent<AudioSource>();
        effectSource.playOnAwake = false;
        effectSource.volume = 0.5f;
    }

    /// <summary>
    /// 设置背景音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    /// <summary>
    /// 获取当前背景音量
    /// </summary>
    /// <returns></returns>
    public float GetBgmVolume()
    {
        return bgmSource.volume;
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetEffectVolume(float volume)
    {
        effectSource.volume = volume;
    }

    /// <summary>
    /// 获取当前音效音量
    /// </summary>
    /// <returns></returns>
    public float GetEffectVolume()
    {
        return effectSource.volume;
    }

    /// <summary>
    /// 播放指定背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBgm(string name)
    {
        if(audioClipsDic.TryGetValue(name,out AudioClip clip))
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            ResMgr.LoadAsset<AudioClip>("audio", new string[] { name }, (obj) =>
               {
                   AudioClip newClip = obj[0] as AudioClip;
                   audioClipsDic.Add(name, newClip);
                   bgmSource.clip = newClip;
                   bgmSource.Play();
               });
        }
    }

    /// <summary>
    /// 播放指定音效
    /// </summary>
    /// <param name="name"></param>
    public void PlayEffect(string name)
    {
        if(audioClipsDic.TryGetValue(name,out AudioClip clip))
        {
            effectSource.clip = clip;
            effectSource.Play();
        }
        else
        {
            ResMgr.LoadAsset<AudioClip>("audio", new string[] { name }, (obj) =>
            {
                AudioClip newClip = obj[0] as AudioClip;
                audioClipsDic.Add(name, newClip);
                effectSource.clip = newClip;
                effectSource.Stop();
                effectSource.Play();
            });
        }
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBgm()
    {
        bgmSource.Pause();
    }

    /// <summary>
    /// 暂停音效
    /// </summary>
    public void PauseEffect()
    {
        effectSource.Pause();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBgm()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopEffect()
    {
        effectSource.Stop();
    }

}
