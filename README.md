# SampleCode

현재 진행 중인 프로젝트에서 일부 스크립트를 첨부하였습니다.
여러 식물을 관리하는 매니저 역할의 Plant.cs와 각 식물의 상태를 관리하는 PlantSlot.cs입니다.

Plant.cs는 각각의 식물의 데이터를 관리합니다.
데이터는 BGDatabase로 테이블을 만들어 관리하고 있으며, 팝업창을 띄워 식물을 심을 수 있도록 구현했습니다.
IPointer Interface로 터치 이벤트를 관리했습니다. 
Box Collider를 각 PlantSlot에 넣었으며, 터치된 PlantSlot에 선택한 씨앗을 심을 수 있도록 구현했습니다.

PlantSlot.cs는 식물의 데이터를 Plant에 넘겨주며, 식물의 상태에 따라 Sprite를 관리합니다.

Beaker.cs과 Pot은 포션을 제작하는 과정입니다.
Beaker는 기다리는 시간 없이 조합만으로 바로 원하는 포션을 만들 수 있으며,
Pot은 제작 시간이 필요합니다. 

간단한 주석으로 설명을 달아놨습니다.
