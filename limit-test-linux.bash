for i in {1..25}
do
  printf "Call $i result is "
  curl -s -o /dev/null -w "%{http_code}"  "http://localhost:5008/hotel/city?city=Bangkok"
  printf "\n"
done
