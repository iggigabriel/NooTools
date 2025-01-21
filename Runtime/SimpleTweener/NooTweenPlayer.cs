using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static Noo.Tools.NooTween.NooTween;

namespace Noo.Tools.NooTween
{
    public sealed class NooTweenPlayer : IDisposable
    {
        static readonly ObjectPool<NooTweenPlayer> pool = new(() => new());

        public NooTween Tween { get; private set; }
        public GameObject Target { get; private set; }
        public float Time { get; private set; }
        public int CurrentLoop { get; private set; }
        public int CurrentTrackIndex { get; private set; } = -1;
        public float CurrentTrackTime { get; private set; }
        public float CurrentTrackNormalizedTime { get; private set; }

        readonly Dictionary<(NooTweenTrack track, string name), object> customData = new();

        public T GetCustomData<T>(NooTweenTrack track, string name, T defaultValue)
        {
            if (customData.TryGetValue((track, name), out var value) && value is T tValue) return tValue;
            SetCustomData(track, name, defaultValue);
            return defaultValue;
        }

        public T GetCustomData<T>(NooTweenTrack track, string name, Func<T> valueGetter = default)
        {
            if (customData.TryGetValue((track, name), out var value) && value is T tValue) return tValue;
            var defaultValue = valueGetter == null ? default : valueGetter();
            SetCustomData(track, name, defaultValue);
            return defaultValue;
        }

        public void SetCustomData<T>(NooTweenTrack track, string name, T value)
        {
            customData[(track, name)] = value;
        }

        private void Clear()
        {
            if (CurrentTrackIndex == -1) return;

            if (Tween != null) foreach (var track in Tween.sequence) track?.Clear(this);

            Tween = default;
            Target = default;
            Time = 0;
            CurrentLoop = 0;
            CurrentTrackIndex = -1;
            CurrentTrackTime = 0;
            CurrentTrackNormalizedTime = 0;
            customData.Clear();
        }

        public static NooTweenPlayer GetNew(NooTween tween, GameObject target)
        {
            var player = pool.Get();
            player.Tween = tween;
            player.Target = target;
            return player;
        }

        public void Dispose()
        {
            Clear();
            pool.Release(this);
        }

        public void Update(float deltaTime)
        {
            if (Tween == null || !Target || Tween.sequence.Count == 0) return;

            if (CurrentTrackIndex == -1)
            {
                foreach (var track in Tween.sequence) track?.Init(this);
                PlayNext();
            }

            if (CurrentTrackIndex >= Tween.sequence.Count) return;

            var currentTrack = Tween.sequence[CurrentTrackIndex];

            if (currentTrack == null)
            {
                PlayNext();
            }
            else
            {
                Time += deltaTime;
                CurrentTrackTime += deltaTime;

                currentTrack.Evaluate(this, currentTrack.GetNormalizedTime(CurrentTrackTime));

                if (CurrentTrackTime >= currentTrack.TotalDuration)
                {
                    PlayNext();
                }
            }
        }

        private void PlayNext()
        {
            CurrentTrackIndex++;
            CurrentTrackTime = 0f;

            if (CurrentTrackIndex >= Tween.sequence.Count)
            {
                if (Tween.repeat == RepeatBehaviour.None)
                {
                    return;
                }
                else if (Tween.repeat == RepeatBehaviour.Repeat)
                {
                    CurrentTrackIndex = 0;
                    CurrentLoop++;
                }
            }

            var track = Tween.sequence[CurrentTrackIndex];

            if (track != null)
            {
                track.Start(this);
                track.Evaluate(this, track.GetNormalizedTime(0f));
            }
        }
    }
}
