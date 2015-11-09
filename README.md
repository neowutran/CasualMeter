# CasualMeter
[CasualMeter] is a free open-source damage meter based off [TeraDamageMeter].  The majority of the new stuff is UI and UX improvements, but there are some bug fixes as well.  This [fork] is also worth mentioning since I borrowed the dps paste and settings storage from there.

### Features

Here are some of the features that are currently implemented:
* View damage dealt, damage healed, dps, total damage
* View detailed skill breakdown with three different views
* Will only show up while you are playing Tera
* Paste dps
* Customizable hotkeys (through settings file)

### Roadmap for major features
* Save list of encounters to review
* TBD

### Third-Party libraries

* [MvvmLight] - MVVM Framework
* [Hardcodet] - WPF Taskbar Notification Icon
* [log4net] - Logging
* [Nicenis] - INotifyProperyChanged implementation
* [Gma.UserActivityMonitor] - Global hotkeys
* [Newtonsoft] - Json serializer/deserializer

### Installation

* http://casualmeter.azurewebsites.net/install/publish.htm

### Development

You may contribute by submitting a pull request for bugfixes/updates between patches. A guide to updating opcodes (when it breaks after a patch) can be found [here]. In case it goes down, here's an [imgur mirror].  Pastebins: [1] and [2]

License
----

MIT



[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)

   [CasualMeter]: <https://github.com/lunyx/CasualMeter>
   [MvvmLight]: <http://www.mvvmlight.net/>
   [Hardcodet]: <http://www.hardcodet.net/wpf-notifyicon>
   [log4net]: <https://logging.apache.org/log4net/>
   [Nicenis]: <https://nicenis.codeplex.com/>
   [Gma.UserActivityMonitor]: <http://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C>
   [Newtonsoft]: <http://www.newtonsoft.com/json>
   [TeraDamageMeter]: <https://github.com/gothos-folly/TeraDamageMeter>
   [fork]: <https://github.com/bonekid/TeraDamageMeter>
   [here]: <https://forum.ragezone.com/f797/release-tera-live-packet-sniffer-1052922/index2.html#post8369480>
   [imgur mirror]: <http://i.imgur.com/VTaWEe9.png>
   [1]: <http://pastebin.com/qTGzrW8w>
   [2]: <http://pastebin.com/BTu7mm5C>
