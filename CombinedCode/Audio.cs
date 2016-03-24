using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace CombinedCode
{
    class Audio
    {
        ContentManager Content;

        Dictionary<string, SoundEffect> soundDict;
        public Audio(ContentManager c)
        {
            soundDict = new Dictionary<string, SoundEffect>();
            Content = c;
        }

        public void LoadSounds()
        {
            soundDict.Add("Gunshot",
                this.Content.Load<SoundEffect>("Audio/M4A1_Shot"));
           
            soundDict.Add("Wilhelm",
                this.Content.Load<SoundEffect>("Audio/Wilhelm"));

            soundDict.Add("Mein Leben!",
                this.Content.Load<SoundEffect>("Audio/MeinLeben"));
        }

        public void PlaySoundEffect(string sfxName)
        {
            if(soundDict.ContainsKey(sfxName))
            {
                soundDict[sfxName].Play();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Couldn't find sound file: " + sfxName);
            }
        }
    }
}
