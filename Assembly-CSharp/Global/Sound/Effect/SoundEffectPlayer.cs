﻿using System;
using System.Collections.Generic;

public class SoundEffectPlayer : SoundPlayer
{
	public void SetVolume(Single volume)
	{
		SoundDatabase[] array = new SoundDatabase[]
		{
			this.gameSoundDatabase,
			this.sceneSoundDatabase,
			this.onTheFlySoundDatabase
		};
		SoundDatabase[] array2 = array;
		for (Int32 i = 0; i < (Int32)array2.Length; i++)
		{
			SoundDatabase soundDatabase = array2[i];
			foreach (KeyValuePair<Int32, SoundProfile> keyValuePair in soundDatabase.ReadAll())
			{
				SoundProfile value = keyValuePair.Value;
				Int32 soundID = value.SoundID;
				if (this.playedEffectSet.Contains(soundID))
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(soundID, volume, 0);
					SoundLib.Log("Set volume to soundID: " + soundID + " finished");
				}
				else
				{
					SoundLib.Log("soundID: " + soundID + " not found");
				}
			}
		}
		this.playerVolume = volume;
	}

	public void StopAllSoundEffects()
	{
		SoundDatabase[] array = new SoundDatabase[]
		{
			this.gameSoundDatabase,
			this.sceneSoundDatabase,
			this.onTheFlySoundDatabase
		};
		SoundDatabase[] array2 = array;
		for (Int32 i = 0; i < (Int32)array2.Length; i++)
		{
			SoundDatabase soundDatabase = array2[i];
			foreach (KeyValuePair<Int32, SoundProfile> keyValuePair in soundDatabase.ReadAll())
			{
				SoundProfile value = keyValuePair.Value;
				Int32 soundID = value.SoundID;
				if (this.playedEffectSet.Contains(soundID))
				{
					ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundID, 0);
					this.playedEffectSet.Remove(soundID);
					SoundLib.Log("Force stop success");
				}
				else
				{
					SoundLib.Log("soundID: " + soundID + " not found");
				}
			}
		}
	}

	public Boolean IsSoundEffectPlaying(Int32 soundIndex)
	{
		SoundDatabase[] array = new SoundDatabase[]
		{
			this.gameSoundDatabase,
			this.sceneSoundDatabase,
			this.onTheFlySoundDatabase
		};
		SoundDatabase[] array2 = array;
		for (Int32 i = 0; i < (Int32)array2.Length; i++)
		{
			SoundDatabase soundDatabase = array2[i];
			foreach (KeyValuePair<Int32, SoundProfile> keyValuePair in soundDatabase.ReadAll())
			{
				SoundProfile value = keyValuePair.Value;
				if (value.SoundIndex == soundIndex)
				{
					Int32 soundID = value.SoundID;
					if (this.playedEffectSet.Contains(soundID))
					{
						return true;
					}
					SoundLib.Log("soundID: " + soundID + " not found");
				}
			}
		}
		SoundLib.Log("soundIndex: " + soundIndex + " not found in DB");
		return false;
	}

	public void StopSoundEffect(Int32 soundIndex)
	{
		SoundDatabase[] array = new SoundDatabase[]
		{
			this.gameSoundDatabase,
			this.sceneSoundDatabase,
			this.onTheFlySoundDatabase
		};
		SoundDatabase[] array2 = array;
		for (Int32 i = 0; i < (Int32)array2.Length; i++)
		{
			SoundDatabase soundDatabase = array2[i];
			foreach (KeyValuePair<Int32, SoundProfile> keyValuePair in soundDatabase.ReadAll())
			{
				SoundProfile value = keyValuePair.Value;
				if (value.SoundIndex == soundIndex)
				{
					Int32 soundID = value.SoundID;
					if (this.playedEffectSet.Contains(soundID))
					{
						ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(soundID, 0);
						this.playedEffectSet.Remove(soundID);
						SoundLib.Log("Force stop success");
					}
					else
					{
						SoundLib.Log("soundID: " + soundID + " not found");
					}
				}
			}
		}
	}

	public void PlaySoundEffect(Int32 soundIndex, Single soundVolume = 1f, Single panning = 0f, Single pitch = 1f)
	{
		this.PlaySoundEffect(soundIndex, soundVolume, panning, pitch, SoundProfileType.SoundEffect);
	}

	protected void PlaySoundEffect(Int32 soundIndex, Single soundVolume = 1f, Single panning = 0f, Single pitch = 1f, SoundProfileType type = SoundProfileType.SoundEffect)
	{
		SoundProfile soundProfile = this.gameSoundDatabase.Read(soundIndex);
		if (soundProfile == null)
		{
			soundProfile = this.sceneSoundDatabase.Read(soundIndex);
		}
		if (soundProfile == null)
		{
			soundProfile = this.onTheFlySoundDatabase.Read(soundIndex);
		}
		if (soundProfile != null)
		{
			soundProfile.SoundVolume = soundVolume * this.playerVolume;
			soundProfile.Panning = panning;
			soundProfile.Pitch = pitch;
			this.activeSoundEffect = soundProfile;
			this.PlaySoundEffect(soundProfile);
		}
		else
		{
			SoundLib.Log(String.Empty + soundIndex + " is not exist");
			soundProfile = SoundMetaData.GetSoundProfile(soundIndex, type);
			soundProfile.SoundVolume = soundVolume * this.playerVolume;
			soundProfile.Panning = panning;
			soundProfile.Pitch = pitch;
			if (soundProfile == null)
			{
				SoundLib.LogError("soundIndex: " + soundIndex + " is not exist");
				return;
			}
			this.activeSoundEffect = soundProfile;
			if (this.onTheFlySoundDatabase.ReadAll().Count >= 20)
			{
				SoundLib.Log("Unload on the fly sound database.");
				base.UnloadResource(this.onTheFlySoundDatabase);
			}
			base.LoadResource(soundProfile, this.onTheFlySoundDatabase, new SoundPlayer.LoadResourceCallback(this.LoadOnTheFlySoundResourceCallback));
		}
	}

	private void LoadOnTheFlySoundResourceCallback(SoundDatabase soundDatabase, Boolean isError)
	{
		if (!isError)
		{
			if (this.activeSoundEffect != null)
			{
				this.PlaySoundEffect(this.activeSoundEffect);
			}
		}
		else
		{
			SoundLib.Log("LoadOnTheFlySoundResourceCallback is Error");
		}
	}

	private void PlaySoundEffect(SoundProfile soundProfile)
	{
		base.CreateSound(soundProfile);
		base.StartSound(soundProfile, 1f);
		this.playedEffectSet.Add(soundProfile.SoundID);
		soundProfile.SoundProfileState = SoundProfileState.Released;
		this.gameSoundDatabase.Update(soundProfile);
	}

	public void LoadGameSoundEffect(String metaData)
	{
		base.UnloadResource(this.gameSoundDatabase);
		base.LoadResource(metaData, this.gameSoundDatabase, new SoundPlayer.LoadResourceCallback(this.LoadGameSoundResourceCallback));
	}

	private void LoadGameSoundResourceCallback(SoundDatabase soundDatabase, Boolean isError)
	{
		SoundLib.Log("LoadGameSoundResourceCallback: " + ((!isError) ? "Success" : "Error"));
	}

	public void LoadSceneSoundEffect(String metaData)
	{
		base.UnloadResource(this.sceneSoundDatabase);
		base.LoadResource(metaData, this.sceneSoundDatabase, new SoundPlayer.LoadResourceCallback(this.LoadSceneSoundResourceCallback));
	}

	private void LoadSceneSoundResourceCallback(SoundDatabase soundDatabase, Boolean isError)
	{
		SoundLib.Log("LoadSceneSoundResourceCallback: " + ((!isError) ? "Success" : "Error"));
	}

	public void UnloadSoundEffect()
	{
		base.UnloadResource(this.onTheFlySoundDatabase);
		base.UnloadResource(this.sceneSoundDatabase);
	}

	public void UnloadAllSoundEffect()
	{
		base.UnloadResource(this.onTheFlySoundDatabase);
		base.UnloadResource(this.sceneSoundDatabase);
		base.UnloadResource(this.gameSoundDatabase);
	}

	public override void Update()
	{
		foreach (Int32 num in this.playedEffectSet)
		{
			if (ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsExist(num) == 0)
			{
				ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(num, 0);
				this.playedEffectRemoveList.Add(num);
				SoundLib.Log("Sound End, Stop success");
			}
		}
		foreach (Int32 item in this.playedEffectRemoveList)
		{
			if (!this.playedEffectSet.Remove(item))
			{
				SoundLib.Log("Remove playedEffectSet failure!");
			}
		}
		this.playedEffectRemoveList.Clear();
	}

	private SoundDatabase gameSoundDatabase = new SoundDatabase();

	private SoundDatabase sceneSoundDatabase = new SoundDatabase();

	private SoundDatabase onTheFlySoundDatabase = new SoundDatabase();

	private HashSet<Int32> playedEffectSet = new HashSet<Int32>();

	private List<Int32> playedEffectRemoveList = new List<Int32>();

	private SoundProfile activeSoundEffect;

	private Single playerVolume = 1f;
}
