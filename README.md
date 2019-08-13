# IETU.Winforms - Image Enhancing Training Utility
Windows only GUI for training BasicSR (ESRGAN, PPON) models

Please, before using this tool ensure that you installed all python dependencies correctly and BasicSR training is working on it's own.

## Requirements

* .NET Framework 4.6.1
* [victorca25's BasicSR fork](https://github.com/victorca25/BasicSR)

## Usage
Set BasicSR-master root path, paths for dataset HR and LR if they differ from default ones. If you want to use pretrained model, set it's path and uncheck Disable flag, same goes to resume state.

Change training settings how you like them. For detailed description for OTF (on the fly) dataset augmentations see [Dataset Augmentation](https://github.com/victorca25/BasicSR/wiki/Dataset-Augmentation).

**[Use HR as LR]** setting sets both Validation and Train LR paths same as HR.

Changing **[Network]** value automatically changes network-specific settings in config.

If you want change more settings go to Advanced config tab.

All current setting can be saved as json config with specified name in main tab. You can then load/delete selected config. 

### Config
![config](https://i.imgur.com/Iu7fWPZ.png)
### Main tab
![main_tab](https://i.imgur.com/Q2s8LhW.png)
### Advanced config
![config](https://i.imgur.com/JMdqAGy.png)
