# LocalizationEditor

    [패치노트]
    
            v1.0.0  - 24.04.02
                    - Localization Editor 기능 구현

---------------------------------------------------------------------------------------------
![image](https://github.com/kastro723/LocalizationEditor/assets/55536937/30ea5a25-1d23-4458-bd5d-f3a1bdc7eb84)
![image](https://github.com/kastro723/LocalizationEditor/assets/55536937/d7f27a70-473d-4e75-9849-4d85b4386e71)


    [기능설명]
    
            Excel파일 (.xslx)의 첫 번째 열을 대문자로 상수로 선언하고, 열을 값으로 받는 상수 키 클래스(LocalizationConstants.cs) 자동 생성

            LocalizationManger 싱글톤 클래스를 통해(instance) 해당 상수 키를 받아 해당하는 문자열 반환
            
            LocalizationManager 클래스를 통한 언어(kr,en,jp) 변경 지원


    [사용방법]
    
            1. Drag and Drop을 통해 Excel(.xlsx)파일을 DragArea에 넣는다.

            2. 'Generate Constants Class' 버튼을 클릭해 저장 폴더를 선택 및 스크립트 자동 생성

            3. LocalizationManager 클래스를 통해 언어 설정 및 상수 키를 입력해 현재 설정된 언어와 상수 키에 맞는 매칭 값(문자열) 리턴


​            
