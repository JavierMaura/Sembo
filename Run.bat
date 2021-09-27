c:
md c:\SemboJMaura
cd c:\SemboJMaura
git clone https://github.com/JavierMaura/Sembo.git

cd Sembo

cd Sembo
start dotnet run --urls "http://localhost:5100;https://localhost:44364"

timeout 5

cd ..\SemboBlazorClient
start dotnet run --urls "http://localhost:44399;https://localhost:44392"

cd..

timeout 10
start "" https://localhost:44392