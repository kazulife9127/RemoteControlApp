package main

import (
	"fmt"
	"net/http"
)

func main() {
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintf(w, "こんにちは！GolangとDockerで作られたアプリです！")
	})

	fmt.Println("サーバー起動 :8080")
	http.ListenAndServe(":8080", nil)
}
