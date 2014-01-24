@echo off

%~dp0..\Python\App\python.exe "%~dp0playgame.py" ^
 --engine_seed 90 ^
 --player_seed 90 ^
 --end_wait=10 ^
 --verbose ^
 --log_dir game_logs ^
 --turntime=20000000 ^
 --loadtime=1000000 ^
 --turns 1000 ^
 --map_file "%~dp0maps\Practical2\cross15.map" ^
 "%~dp0..\Practical2Bot\bin\Release\MyBot.exe" ^
 "%~dp0..\Python\App\python.exe ""%~dp0sample_bots\dommebot\MyBot.py3"""