# 학습 프로젝트
안드로이드 어플리케이션 자동 배포 시스템 구축

# 프로세스 과정
```
1. GitHub로 Commit을 Push
2. GitHub에서 Action 트리거 발동 (push, tag, merge 등)
3. Action에서 Unity 프로젝트 빌드
4. 빌드된 파일 PlayStore로 배포
```
![FlowChart](https://user-images.githubusercontent.com/37904040/178136895-53b38fb8-b139-46f2-be93-e169b53f430a.png)

# 주요 모듈
- [Unity Builder (game-ci/unity-builder)](https://github.com/marketplace/actions/unity-builder)
- [Upload Play Store (r0adkll/upload-google-play)](https://github.com/marketplace/actions/upload-android-release-to-play-store)

# 배포 상황 [#GooglePlay](https://play.google.com/store/apps/details?id=com.Normals.SimpleBall&hl=ko)
![image](https://user-images.githubusercontent.com/37904040/178137134-8dbac23f-dbb8-4964-a27a-0ff55f0ffa16.png)