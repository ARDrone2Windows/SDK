Welcome to our ARDrone2Windows Web site.
===

This SDK shared “As Is” lets you control your Parrot AR.Drone for either a Windows 8 tablet or a Windows Phone 8. It is composed of one Windows Phone 8 and one Windows 8 library that share 95% of their code. 
We also provide two samples apps (one for Windows 8 and one for Windows Phone 8) to demonstrate the usage of our SDK.

### Content
In the current version of our SDK you will find
- Core components to control the drone from Windows 8 or Windows Phone 8.
- Input controls to send commands trough virtual or concrete XBox joysticks.
- A Navigation data reader
- A Configuration data reader and writer
- A video component to render the stream video on both Windows 8 and Windows Phone 8.
- A FTP client to download images 

#### Windows app
We are proud to share a first beta version of Free Flight for Windows 8.
Feel free to reuse this code to unleash your creativity. 
<img src="https://github.com/ARDrone2Windows/SDK/blob/master/Images/Windows_Surface_FF2_01.png?raw=true" />

#### Windows Phone app
We propos a simple sample app, to demonstrate the usage of our SDK on Windows Phone.

### Known issues
Despite our efforts to propose to the community a well-polished SDK, we apologize if you won’t get the best experience from our work. Please [log any bug or improvement request](https://github.com/ARDrone2Windows/SDK/issues/new) that may help us to improve the quality of the solution. We will update both the SDK and its [documentation](https://github.com/ARDrone2Windows/SDK/wiki) from your feedback. We don’t have the pretention neither to release a zero bug solution, nor to cover all the features supported by the AR.Drone 2.0. 
Here are the bugs/unfinished work we will try to solve in our next version:
- Joystick controls in multitouch don't return into their default position. Need to touch once again the screen.
- Live video is over during video recording. The live video switch from H264 to MP4 codecs when we the drone starts the video recording. You currently experience a black screen because we didn't implement the MP4 decoder yet.
- Fake content in the WIndows app. We introduces some fake content in the Windows app, to present our vision of the final release (an idea of the work in progress) to get your feedback.

### Thanks!!!
This SDK would have never been there without the passion of some valuable guys. We started the development of this SDK reusing most of patterns introduced by [Ruslan Balanukhin]( https://github.com/Ruslan-B) in the great [AR.Drone projet for C# and Mono](https://github.com/Ruslan-B/AR.Drone). Some other great ideas, introduced by [alex100990](http://www.codeplex.com/site/users/view/alex100990) and his team in the [Windows Phone AR.Drone controller]( http://wp7ardrone.codeplex.com/) have been partially reused. Finally thanks to [TechPreacher](https://github.com/TechPreacher) who has shared a nice [Joystick control] (https://github.com/TechPreacher/WP8-Joystick-UserControl), we reused on improved in our code.
At your turn, feel free to download this SDK, extend, test, send feedback and finally share!

Enjoy the experience!
