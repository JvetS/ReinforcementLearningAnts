@echo off
%~dp0..\Python\App\python.exe "%~dp0playgame.py" ^
 --engine_seed 42 ^
 --player_seed 42 ^
 --end_wait=5 ^
 --rounds 100 ^
 --log_dir game_logs ^
 --turntime=20000000 ^
 --loadtime=1000000 ^
 --turns 1000 ^
 --nolaunch ^
 --map_file "%~dp0maps\practical2\cross15.map" ^
 "%~dp0..\Practical2Bot\bin\Debug\MyBot.exe" ^
 "%~dp0..\Python\App\python.exe ""%~dp0sample_bots\dommebot\MyBot.py3"""
