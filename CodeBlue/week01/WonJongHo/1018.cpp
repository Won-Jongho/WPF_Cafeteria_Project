#include <iostream>
#include <string>
using namespace std;

int main() {
	int N = 0, M = 0;
	string board[51];

	int cnt[2] = { 0 }, min = 64;
	bool sw = true;

	cin >> N >> M;
	cin.ignore();

	for (int i = 0; i < N; i++) {
		getline(cin, board[i]);
	}

	for (int i = 0; i < N - 7; i++) {
		for (int j = 0; j < M - 7; j++) {
			cnt[0] = 0;
			cnt[1] = 0;
			for (int k = i; k < i + 8; k++) {
				for (int l = j; l < j + 8; l++) {
					if (sw == true) {
						if (board[k][l] == 'B') {
							cnt[0] += 1;
						}
						else {
							cnt[1] += 1;
						}
					}
					else {
						if (board[k][l] == 'W') {
							cnt[0] += 1;
						}
						else {
							cnt[1] += 1;
						}
					}
					sw = !sw;
				}
				sw = !sw;
			}
			for (int a = 0; a < 2; a++) {
				if (min > cnt[a]) {
					min = cnt[a];
				}
			}
		}
	}
	cout << min;
	return 0;
}
