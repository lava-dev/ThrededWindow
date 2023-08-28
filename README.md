Purpose and Problem Description 
 
I’m an experienced WPF/.NET developer. And everyone who works with that amazing technology knows that there are a lot of backside things which unfortunately don’t have right or good solutions. I’d like to tell you about this kind of exceptional case.
 
This time, I was developing a desktop application and on the right top of the working window, I have to put the EKG gif (The Graphics Interchange Format, which is by simple words just animated image). This EKG should show you that the application doesn’t stick, that it’s alive, and gives a good feeling to the user who is working with it.
 
So, before, I had some experience with that kind of apps. Of course, I implemented everything together, don’t mind and sometimes it stuck.
 
A little step to left. When you work with a big amount of data in WPF that kind things unfortunately happen. Because, when you are sending data to the UI the WPF-renderer at the moment stuck and all your live objects stopped moving, which makes a bad impression for users. Of course, there are existing solutions, that can work for your needs for some point: you can send it partially (old school solution using BackgroundWorker), using virtualization (which is also far from an ideal solution), and so on. Always controlling these things is annoying me, I’d say.
 
That’s why I started to think on some better solution which could make this EKG animation works totally independent from the rest of app processes. Finally, I found it, but on my way, I met a lot of difficulties and showstopper cases.
