# This will build and copy the screensaver to your Windows directory
# so you can test integration locally.
# 
# Obviously this must be run with admin permissions, which you can 
# do with an elevated command prompt or the 'sudo' command.

dotnet publish -p:PublishProfile=Deploy.pubxml
copy-item .\bin\Release\net10.0-windows\win-x64\publish\yoksdotnet.exe C:\Windows\System32\yoksdotnet.scr