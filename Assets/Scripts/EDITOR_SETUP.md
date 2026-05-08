# Trebuchet MVP — Editor Setup (подробно)

Это пошаговая инструкция для Unity Editor 6 (URP). Каждый шаг — что
сделать, где это в интерфейсе, что должен увидеть, типичные ошибки.

> Обозначения:
> - **жирным** — то, на что кликаешь / что выбираешь.
> - `моноширинно` — имена ассетов, типов, методов и значения полей.
> - 🔍 — что должно появиться в результате.
> - ⚠️ — частая ошибка / на что обратить внимание.

---

## Перед началом

1. Открой проект `Counterweight` в Unity Hub. Подожди, пока Unity закончит
   импорт всех новых `.cs` файлов. Внизу справа должен исчезнуть прогресс-бар.
2. Открой консоль: **Window → General → Console**. Должна быть пуста.
   Если есть **красные ошибки компиляции** — остановись и пришли мне их.
   Дальше двигаться нельзя, пока скрипты не компилируются.
3. Проверь, что отображаются 4 сборки. **Project window → Assets/Scripts**:
   увидишь 4 папки `Counterweight.Core`, `Counterweight.Input`,
   `Counterweight.Trebuchet`, `Counterweight.Tests`. В каждой — `.asmdef`.

---

## Шаг 1. Сгенерировать C#-класс из Input Actions

`InputBridge.cs` в коде ссылается на тип `InputSystem_Actions` — Unity
сгенерирует его из `.inputactions` файла, если включить опцию.

1. **Project window** → открой папку `Assets`.
2. Кликни **один раз** по файлу `InputSystem_Actions.inputactions`.
3. В **Inspector** (правая панель) поставь галочку **Generate C# Class**.
4. Под галочкой появятся три поля. Оставь:
   - **C# Class File**: `Assets/InputSystem_Actions.cs` (по умолчанию).
   - **C# Class Name**: `InputSystem_Actions`.
   - **C# Class Namespace**: пусто.
5. Нажми **Apply** (большая кнопка внизу Inspector).

🔍 В `Assets/` появится файл `InputSystem_Actions.cs`. Unity снова
начнёт компиляцию. Дождись окончания. Консоль — без ошибок.

⚠️ Если имя класса другое — `InputBridge.cs` не скомпилируется. Должно
быть ровно `InputSystem_Actions`.

⚠️ В `git status` ты уже видишь, что эти файлы существуют (`InputSystem_Actions.cs`
и `.meta`). Если Generate C# Class **уже включён** — пропусти шаг и переходи к 2.

---

## Шаг 2. Настроить FBX-импорт и нарезать клипы анимации

1. В **Project window** разверни `Assets/_3Dmodels/Trebuchet`.
2. Кликни по `Trebuchet_LODG.fbx`.
3. В **Inspector** появятся вкладки: **Model**, **Rig**, **Animation**, **Materials**.

### 2a. Вкладка Model
1. Открой **Model**.
2. **Scale Factor**: `1` (если модель в Blender — обычно ок). Если требушет
   слишком большой/маленький — сюда вернёшься.
3. Жмёшь **Apply** только если что-то менял.

### 2b. Вкладка Rig
1. Открой **Rig**.
2. **Animation Type**: убедись, что **Generic** (а не Humanoid).
3. **Avatar Definition**: выбери **Create From This Model**.
4. **Root node**: `<None>` (Generic без root motion — корнем будет сам объект).
5. Нажми **Apply**.

🔍 В дочерних элементах FBX в Project появится sub-asset с иконкой
человечка — это Avatar.

### 2c. Вкладка Animation — ключевая
1. Открой **Animation**.
2. Убедись, что **Import Animation** включено. Если нет — включи и **Apply**.
3. Прокрути до раздела **Clips**.

В **Clips** уже должен быть один автогенерированный клип (часто называется
именем такта или `Take 001`). Его длина — длина всей анимации в FBX.

#### 2c.i. Найти границы кадров
1. Кликни на единственный клип в списке.
2. Внизу Inspector есть **превью** — окно с моделью.
3. Под моделью — таймлайн с двумя ползунками **Start / End** (или поля
   Start Frame / End Frame).
4. Прокручивая ползунок таймлайна, найди:
   - **кадр T0** — модель в покое (Idle).
   - **кадр T1** — модель «взведена» (контрвес наверху).
   - **кадр T2** — момент схода снаряда (праща раскрывается, мяч улетает).
   - **кадр T3** — модель остановилась после качания.
   Запиши номера T0, T1, T2, T3 на бумажке. Они понадобятся.

⚠️ Если анимация в FBX включает только сам полёт стрелы (без подъёма
контрвеса) — смотри в конце документа раздел **«Если анимация только Fire»**.

#### 2c.ii. Создать клип Trebuchet_Idle
1. Над списком Clips нажми **+**.
2. Появится новая запись. Кликни на неё.
3. Поля:
   - **Name**: `Trebuchet_Idle`.
   - **Start**: T0 (или T0+1 если хочешь чуть шире).
   - **End**: T0+1 (одного кадра достаточно — это статичная поза).
   - **Loop Time**: ✅ **включи**.
   - **Loop Pose**: ✅ включи.
   - **Cycle Offset**: 0.
   - В блоке **Root Transform Rotation**: **Bake Into Pose** ✅.
   - В блоке **Root Transform Position (Y)**: **Bake Into Pose** ✅.
   - В блоке **Root Transform Position (XZ)**: **Bake Into Pose** ✅.

#### 2c.iii. Создать клип Trebuchet_Fire
1. Снова нажми **+**.
2. Поля:
   - **Name**: `Trebuchet_Fire`.
   - **Start**: T1 (или T0, если хочешь, чтобы Fire начинался от Idle).
   - **End**: T3.
   - **Loop Time**: ❌ **выключи**.
   - **Root Transform** во всех трёх блоках: **Bake Into Pose** ✅.

3. Нажми **Apply** (важно — без этого клипы не сохранятся).

🔍 В Project под `Trebuchet_LODG.fbx` появятся два sub-asset с иконкой
плёнки: `Trebuchet_Idle` и `Trebuchet_Fire`. Раскрой стрелочку слева
от FBX — увидишь их.

⚠️ Если **+** не нажимается, или клипы не появляются — проверь, что
жмёшь **Apply**, а не выбираешь другой объект.

---

## Шаг 3. Animation Event `OnProjectileRelease` на кадре схода снаряда

Этот шаг — самый «снайперский». От него зависит точность вылета снаряда.

1. В Project выбери sub-asset `Trebuchet_Fire` (под FBX).
2. Открой **Window → Animation → Animation** (горячая клавиша `Ctrl+6`).
3. Если окно говорит «To begin animating, create an Animator…» — это норм,
   мы редактируем готовый клип. Окно должно показывать таймлайн клипа
   `Trebuchet_Fire`.
4. **Замок** в углу окна Animation **закрой** (lock icon) — чтобы при
   клике в Project клип не сменился.
5. На таймлайне есть «играть»-кнопка. Нажми, посмотри анимацию.
6. Найди **точный кадр**, где праща раскрывается и снаряд должен сойти —
   обычно в самом конце махового движения, когда длинное плечо смотрит
   почти вперёд-вверх.
7. Поставь **time cursor** (вертикальную линию) на этот кадр. Слева
   указывается номер кадра — запомни его (T2).
8. На панели окна Animation есть кнопка **Add Event** (иконка похожа на
   флажок или прямоугольник с двумя ножками). Нажми её.
9. На таймлайне появится белый маркер. Кликни по нему дважды (или
   правой кнопкой → **Edit Animation Event**).
10. Откроется попап:
    - **Function**: введи **ровно** `OnProjectileRelease` (с большой O,
      без скобок, без аргументов).
    - Все остальные поля (Float, Int, String, Object) оставь пустыми.
11. Закрой попап (✕ или Enter).

🔍 Маркер event на таймлайне должен остаться. При наведении — показывает
имя `OnProjectileRelease`.

⚠️ Если в попапе появляется warning «Function not supported» —
это нормально. Unity ещё не знает, на каком GameObject будет вызов.
Когда мы повесим `TrebuchetAnimationRelay` на префаб, метод найдётся.

⚠️ Имя функции **чувствительно к регистру**. Опечатка → событие никогда
не сработает.

---

## Шаг 4. Создать ScriptableObject ассеты

### 4a. Папка
1. **Project window** → правый клик по `Assets` → **Create → Folder**.
2. Имя: `ScriptableObjects`.

### 4b. TrebuchetConfig_Default
1. Открой `Assets/ScriptableObjects`.
2. Правый клик в пустой области → **Create → Counterweight → Trebuchet Config**.
   ⚠️ Если такого пункта нет — значит скрипт `TrebuchetConfig.cs` не скомпилировался.
   Вернись и проверь Console.
3. Имя ассета: `TrebuchetConfig_Default`.
4. Кликни по нему. В Inspector:
   - `Counterweight Mass` = 200
   - `Arm Length` = 4
   - `Sling Length` = 3
   - `Release Angle Deg` = 45
   - `Launch Efficiency` = 0.55
   - `Projectile Mass` = 8
5. Эти значения уже стоят по умолчанию — менять не надо.

### 4c. ProjectileConfig_Stone
1. В той же папке: правый клик → **Create → Counterweight → Projectile Config**.
2. Имя: `ProjectileConfig_Stone`.
3. Inspector:
   - `Prefab`: пока пусто. Заполним в Шаге 5.
   - `Mass` = 8
   - `Linear Damping` = 0.05
   - `Angular Damping` = 0.05

---

## Шаг 5. Собрать `StoneProjectile.prefab`

### 5a. Папка
1. Правый клик по `Assets` → **Create → Folder** → `Prefabs`.
2. Внутри `Prefabs` ещё одна → `Projectiles`.

### 5b. GameObject
1. Открой любую сцену (например `SampleScene`).
2. **Hierarchy** → правый клик → **3D Object → Sphere**. Появится `Sphere`.
3. В Inspector:
   - **Transform** → Scale: `(0.4, 0.4, 0.4)`.
4. Удали стандартный материал, если хочешь — но не обязательно.

### 5c. Компоненты
1. С выделенной `Sphere` в Inspector нажми **Add Component**.
2. Введи `Rigidbody`, выбери. Появится Rigidbody.
   - **Mass**: оставь 1 (всё равно перезапишется из `ProjectileConfig`).
   - **Use Gravity**: ✅.
   - **Is Kinematic**: ❌.
3. **Sphere Collider** уже есть автоматически. Не трогаем.
4. **Add Component** → введи `Projectile`. Должен появиться скрипт
   `Counterweight.Trebuchet.Projectile`. Добавь его.
   ⚠️ Если не находится — скрипты не скомпилировались.

### 5d. Превратить в префаб
1. Перетащи `Sphere` из **Hierarchy** в `Assets/Prefabs/Projectiles`.
2. В диалоге выбери **Original Prefab**.
3. Переименуй файл в Project: `StoneProjectile`.
4. Удали `Sphere` из Hierarchy сцены (Delete) — он там больше не нужен.

🔍 В Project — синий куб-иконка `StoneProjectile.prefab`.

### 5e. Связать с конфигом
1. Открой `ScriptableObjects/ProjectileConfig_Stone`.
2. В Inspector в поле **Prefab** перетащи `StoneProjectile.prefab` из Project.

---

## Шаг 6. Собрать Animator Controller

### 6a. Папка и контроллер
1. Правый клик по `Assets` → **Create → Folder** → `Animation`.
2. Внутри: правый клик → **Create → Animator Controller** → `TrebuchetAnimator`.

### 6b. Открыть и настроить
1. Двойной клик по `TrebuchetAnimator` — откроется окно **Animator** (граф состояний).
2. Слева вкладка **Parameters**. Нажми **+** → выбери **Trigger** → имя `Fire`.
3. На графе уже есть три встроенных состояния (Entry, Exit, Any State).

### 6c. Состояние Idle
1. Перетащи sub-asset `Trebuchet_Idle` (из FBX в Project) **в окно Animator**.
2. Появится оранжевое (или серое) состояние с именем `Trebuchet_Idle`.
3. Кликни по нему правой кнопкой → **Set as Layer Default State**.
4. Стрелка от **Entry** теперь идёт в `Trebuchet_Idle`.

### 6d. Состояние Firing
1. Перетащи `Trebuchet_Fire` в окно. Появится второе состояние.

### 6e. Переходы
1. **Idle → Firing**:
   - Правый клик по `Trebuchet_Idle` → **Make Transition**. Тяни стрелку до `Trebuchet_Fire`. Кликни.
   - Кликни по только что созданной стрелке. В Inspector:
     - **Has Exit Time**: ❌ (выключи).
     - **Transition Duration (s)**: 0.05.
     - **Conditions**: нажми **+** → выбери `Fire`.
2. **Firing → Idle**:
   - Правый клик по `Trebuchet_Fire` → **Make Transition** → тяни до `Trebuchet_Idle`.
   - В Inspector:
     - **Has Exit Time**: ✅ (включи).
     - **Exit Time**: 0.95.
     - **Transition Duration (s)**: 0.1.
     - **Conditions**: пусто.

🔍 На графе видно две стрелки между Idle и Firing.

---

## Шаг 7. Собрать `Trebuchet.prefab`

### 7a. Положить FBX в сцену
1. Открой сцену `SampleScene` (или любую тестовую).
2. Перетащи `Trebuchet_LODG.fbx` из Project в **Hierarchy** или прямо в **Scene view**.
3. Появится модель требушета. Поставь её в позицию `(0, 0, 0)`.

### 7b. Animator
1. На корне модели в Hierarchy уже **может быть** компонент `Animator` (Unity добавляет автоматически).
   Если нет — **Add Component → Animator**.
2. В компоненте **Animator**:
   - **Controller**: перетащи `TrebuchetAnimator` из Project.
   - **Avatar**: должен быть автоматически выбран (sub-asset Avatar из FBX).
     Если пусто — открой стрелочку слева от FBX в Project, найди Avatar
     с иконкой человечка, перетащи в это поле.
   - **Apply Root Motion**: ❌.
   - **Update Mode**: Normal.
   - **Culling Mode**: Always Animate (чтобы анимация не замирала, когда камера не смотрит).

### 7c. Скрипты на корне
На том же корневом GameObject (где `Animator`):

1. **Add Component → TrebuchetAnimationRelay**. Пустых полей нет.
2. **Add Component → ProjectileSpawner**:
   - **Projectile Config**: перетащи `ProjectileConfig_Stone`.
3. **Add Component → TrebuchetController**:
   - **Config**: перетащи `TrebuchetConfig_Default`.
   - **Animator**: перетащи **этот же GameObject** (или сам Animator-компонент).
   - **Release Point**: пока пусто — заполним в 7d.
   - **Spawner**: перетащи **этот же GameObject** (или сам ProjectileSpawner-компонент).
   - **Animation Relay**: перетащи **этот же GameObject** (или сам Relay-компонент).
   - **Reset Delay Seconds**: 3.

⚠️ Если в полях нужно перетащить компонент именно того типа, который
ожидает поле — Unity сам подсветит подходящий компонент при перетаскивании
GameObject. Если поле остаётся пустым — перетащи компонент напрямую из Inspector
(зацепись за иконку компонента и тяни в нужный slot).

### 7d. ReleasePoint
1. В Hierarchy правый клик по корню требушета → **Create Empty Child**.
2. Переименуй в `ReleasePoint`.
3. Открой окно Animator parameters. В Animation window выбери `Trebuchet_Fire`,
   проскрабь до кадра T2 (момент схода снаряда — тот же, где Animation Event).
4. **В Scene view**: подвинь `ReleasePoint` Transform так, чтобы он
   находился ровно в той точке, где конец пращи в этот кадр (примерно над
   и впереди верхушки длинного плеча).
5. Поверни `ReleasePoint` так, чтобы его **синяя ось Z** смотрела в направлении,
   куда должен лететь снаряд (вперёд требушета, чуть наклонив вниз — но
   наклон угла даст уже `BallisticsSolver`).
6. В Inspector корневого GameObject в `TrebuchetController` → поле **Release Point**
   перетащи только что созданный `ReleasePoint` из Hierarchy.

⚠️ Если ReleasePoint виден внутри модели, а не на конце пращи — снаряд
будет вылетать «изнутри». Время отрегулировать позицию.

⚠️ Координаты — **локальные** относительно требушета. Так prefab будет
работать в любой позиции в мире.

### 7e. Сделать prefab
1. Перетащи корень требушета из Hierarchy в `Assets/Prefabs`.
2. Выбери **Original Prefab**.
3. Имя: `Trebuchet`.
4. Удали инстанс из Hierarchy.

🔍 В `Assets/Prefabs/Trebuchet.prefab` лежит готовый префаб.

---

## Шаг 8. Сцена `FiringRange.unity`

1. **File → New Scene**. В диалоге выбери **Basic (Built-in)** или
   **Standard (URP)** — для URP проекта обычно **Standard**.
2. **File → Save As** → `Assets/Scenes/FiringRange.unity`.

### 8a. Земля
1. **GameObject → 3D Object → Plane**. Появится плоскость.
2. Transform: Position `(0, 0, 0)`, Scale `(20, 1, 20)`.
3. (Опционально) Добавь материал, чтобы было не белое.

### 8b. Trebuchet
1. Перетащи `Assets/Prefabs/Trebuchet.prefab` в Hierarchy.
2. Поставь его на плоскость в точку `(0, 0, 0)`.

### 8c. Камера
1. Если в сцене уже есть **Main Camera** — оставь её. Иначе:
   GameObject → **Camera**, переименуй в `Main Camera`, Tag = `MainCamera`.
2. Выдели Main Camera.
3. Position: примерно `(8, 5, -8)`. Rotation: `(20, -45, 0)` (ориентируйся
   так, чтобы видеть требушет и землю в кадре).
4. **Add Component → DebugFlyCamera**. Параметры по умолчанию ок.

### 8d. InputBridge
1. **GameObject → Create Empty**. Переименуй в `Input`.
2. **Add Component → InputBridge**.

🔍 В сцене четыре корневых объекта: `Plane`, `Trebuchet`, `Main Camera`, `Input`.
Плюс автоматически созданный `Directional Light`.

3. **File → Save** (Ctrl+S).

---

## Шаг 9. Запустить EditMode-тесты

1. **Window → General → Test Runner**. Откроется окно.
2. Сверху две вкладки: **EditMode** и **PlayMode**. Выбери **EditMode**.
3. В дереве слева раскрой `Counterweight.Tests` → `BallisticsSolverTests`.
4. Должно быть 6 тестов:
   - `HeavierCounterweightIncreasesSpeed`
   - `HeavierProjectileDecreasesSpeed`
   - `ZeroEfficiencyProducesZeroSpeed`
   - `ReleaseVelocityHasPositiveYWhenAngleIsPositive`
   - `ReleaseSpeedMatchesEnergyEquation`
   - `NullConfigReturnsZero`
5. Нажми **Run All**.

🔍 Все 6 тестов зелёные.

⚠️ Если вкладки **EditMode** нет — у тебя версия Test Runner устарела
или Tests asmdef не подхватился. Перезапусти Unity.

⚠️ Если тесты не появляются — открой `Counterweight.Tests.asmdef` и
проверь, что галочка `Editor` стоит в Include Platforms (она задаётся
автоматически из json).

---

## Шаг 10. Smoke test

1. В Project открой `Assets/Scenes/FiringRange.unity` (двойной клик).
2. Жми **Play** (треугольник сверху).
3. Что должен увидеть:
   - Требушет стоит, играет `Idle` (визуально либо статичен, либо чуть «дышит»).
   - В Console — никаких ошибок.
4. Кликни **левой кнопкой мыши** один раз в окне Game.
   - В Console: **никаких** ошибок.
   - Требушет начинает Fire-анимацию.
   - В момент схода (твой Animation Event) — на конце пращи появляется
     каменный шар и улетает по дуге.
   - Шар бьётся об землю — в Console появляется лог `[Projectile] Impact with Plane at (...)`.
5. Кликай ещё. Каждые 3 секунды — снова Fire (не чаще, потому что
   `resetDelaySeconds`).
6. Кликни LMB **сразу** во время полёта снаряда — ничего не произойдёт.
   Это правильно: `RequestFire()` игнорируется в состоянии `Firing`/`Released`.

### 10a. Если что-то идёт не так

| Симптом | Что проверить |
|---|---|
| Анимация не играет | Animator Controller на корне, Avatar выставлен, Idle — Default State |
| Анимация играет, но снаряд не вылетает | Animation Event на клипе Fire, имя ровно `OnProjectileRelease`, `TrebuchetAnimationRelay` есть на том же GameObject что Animator |
| Снаряд вылетает не оттуда | Положение `ReleasePoint` |
| Снаряд летит не туда | Поверни `ReleasePoint` — синяя ось Z должна смотреть вперёд |
| Снаряд почти не летит | Открой `TrebuchetConfig_Default`: увеличь `counterweightMass` или `launchEfficiency` |
| Снаряд улетает в космос | Уменьши те же параметры |
| Snap, ничего не происходит при ЛКМ | Объект `Input` с `InputBridge` есть в сцене? Скрипт сгенерирован (Шаг 1)? |
| Console: NullReferenceException | Скорее всего поле в `TrebuchetController` пустое. Проверь все 5 ссылок |
| Console: Receiver class is null… | Animation event в клипе указывает на функцию, которой нет на компонентах — проверь имя `OnProjectileRelease`, без скобок |

---

## Тюнинг после первого успешного выстрела

Все ручки — в `Assets/ScriptableObjects/TrebuchetConfig_Default`:

- **Дальше летит**: больше `counterweightMass` или `armLength`.
- **Выше летит / более дугой**: больше `releaseAngleDeg`.
- **Настильнее**: меньше `releaseAngleDeg` (например 30°).
- **Глобальный feel-knob**: `launchEfficiency`. Реалистичные требушеты —
  около 0.45–0.65.
- **Тяжелее снаряд**: `projectileMass`. Скорость падает квадратично.

Менять можно прямо в Play mode — изменения применятся к следующему
выстрелу. (Но не сохранятся при остановке Play — это известное
поведение Unity.)

---

## Если анимация только Fire (нет Idle)

Если в FBX **нет** статичного Idle — сделай так:

1. На шаге 2c.ii клип `Trebuchet_Idle` сделай длиной 1 кадр в самой
   первой позе анимации (T0=0, End=1, Loop = ON).
2. Дальше всё то же самое.

---

## Если Animator показывает «No Avatar»

1. Открой `Trebuchet_LODG.fbx` → **Rig** → **Avatar Definition** → **Create From This Model** → **Apply**.
2. На префабе требушета: компонент Animator → поле Avatar →
   перетащи sub-asset Avatar (с иконкой человечка из FBX).

---

---

# Iteration 2 — Прицеливание + Trajectory Ghost

В коде уже есть:
- `BallisticsSimulator` — пошаговая симуляция траектории.
- `TrebuchetAimController` — поворот основания + регулировка мощности.
- `TrajectoryRenderer` — `LineRenderer`-призрак.
- Расширение `BallisticsSolver.ComputeReleaseVelocity(..., powerMultiplier)`.

Что нужно сделать в Editor:

## Шаг 11. Добавить `TrebuchetAimController` на префаб

1. Открой `Assets/Prefabs/Trebuchet.prefab` (двойной клик → откроется в режиме редактирования префаба).
2. На корневом GameObject **Add Component → Trebuchet Aim Controller**.
3. Параметры по умолчанию хороши. Можешь подстроить:
   - **Aim Speed Deg Per Sec**: 45 (как быстро поворачивается).
   - **Min/Max Yaw Deg**: ±90.
   - **Min/Max Power**: 0.3 / 1.5.
   - **Starting Power**: 1.
4. На корневом GameObject в `TrebuchetController` появилось поле **Aim Controller** — перетащи туда сам этот же GameObject (или компонент `TrebuchetAimController`).
5. Сохрани префаб (Ctrl+S или кнопка Save в режиме prefab).

⚠️ Aim Controller вращает **тот GameObject, на котором он висит**, вокруг локальной оси Y. Для MVP это корень требушета — приемлемо. Позже, когда добавим раздельную «basement → arm» иерархию, перенесём AimController на basement.

## Шаг 12. Добавить GameObject с `TrajectoryRenderer`

1. Внутри префаба `Trebuchet` создай child: правый клик на корне → **Create Empty** → `TrajectoryGhost`.
2. На нём **Add Component → Line Renderer**. Появятся компоненты `Line Renderer` и сам Transform.
3. Настрой `Line Renderer`:
   - **Width**: поставь 0.05 (тонкая линия) или 0.1.
   - **Material**: создай новый материал → `Assets/Art/M_TrajectoryGhost.mat`. Используй шейдер `Universal Render Pipeline/Unlit`. Цвет — белый или светло-жёлтый, можно с альфой 0.6.
   - **Use World Space**: ✅ (это уже выставит сам скрипт, но проверь).
   - **Texture Mode**: `Tile` или `Stretch` (для пунктира — Tile + текстура с прозрачностями).
4. На том же GameObject **Add Component → Trajectory Renderer**.
5. Настрой ссылки в инспекторе `TrajectoryRenderer`:
   - **Controller**: перетащи корень требушета (где `TrebuchetController`).
   - **Config**: `TrebuchetConfig_Default`.
   - **Aim Controller**: корень требушета (где `TrebuchetAimController`).
   - **Release Point**: `ReleasePoint` из иерархии префаба.
   - **Projectile Config**: `ProjectileConfig_Stone`.
   - **Ground Y**: 0 (если земля у тебя на y=0; иначе подставь её y).
   - **Dt**: 0.05.
   - **Max Points**: 200.
6. Сохрани префаб.

🔍 В Scene view сразу должна появиться кривая, выходящая из ReleasePoint и описывающая дугу до земли.

⚠️ Если линии не видно: открой Game/Scene вид с активным префабом и проверь:
- В `Line Renderer` назначен материал? Без материала линия не рендерится.
- `Width` > 0?
- `Trajectory Renderer.Release Point` назначен?

## Шаг 13. Создать материал для линии (если ещё нет)

1. Создай папку `Assets/Art` если её нет.
2. Правый клик → **Create → Material** → имя `M_TrajectoryGhost`.
3. В Inspector: **Shader → Universal Render Pipeline → Unlit**.
4. **Surface Type**: Transparent.
5. **Base Map** color: например, светло-жёлтый или белый, alpha 0.6.
6. Перетащи материал в slot **Materials → Element 0** в `Line Renderer`.

## Шаг 14. Запустить тесты

1. **Window → General → Test Runner → EditMode → Run All**.
2. Появятся новые тесты в `BallisticsSimulatorTests`. Должны быть зелёные:
   - `StartsAtOrigin`
   - `TrajectoryRisesThenFalls`
   - `HigherPowerLandsFartherWithoutDrag`
   - `StopsAtGround`
   - `NullConfigReturnsZero`
   - `EmptyBufferReturnsZero`
   - И все 6 старых `BallisticsSolverTests` тоже зелёные.

## Шаг 15. Smoke test Iteration 2

1. Открой `FiringRange.unity`, **Play**.
2. Видишь Idle. **Призрак траектории видим** — пунктирная (или сплошная) линия выходит из конца пращи и описывает дугу.
3. **← / →** — основание требушета поворачивается, призрак крутится за ним.
4. **↑ / ↓** или **скролл колесом** — призрак становится длиннее/короче (мощность меняется).
5. **LMB #1** — WindUp. Призрак **исчезает** на момент выстрела (`Firing/Released`).
6. **LMB #2** — Fire. Снаряд летит примерно по той же дуге, где был призрак. Расхождение должно быть в пределах нескольких метров — Rigidbody немного отличается от чистой Эйлеровой интеграции.
7. Через ~3 секунды Idle. Призрак возвращается.

⚠️ Если призрак сильно расходится с реальным полётом:
- Уменьши `Dt` в `TrajectoryRenderer` (например 0.02).
- Убедись, что `Projectile Config` в `TrajectoryRenderer` тот же, что в `ProjectileSpawner`.
- Проверь, что `Linear Damping` в `ProjectileConfig_Stone` действительно отражает то, что Rigidbody получает.

---

# Iteration 3 — FPV персонаж + ритуал интеракций

В коде уже есть:
- **`Counterweight.Player.asmdef`** — новая сборка.
  - `IInteractable` — интерфейс для всего, что игрок может «нажать E».
  - `PlayerController` — CharacterController + WASD + mouse look + cursor lock.
  - `PlayerInteractor` — raycast от камеры, OnGUI HUD (crosshair + prompt).
- **Три интерактивных компонента** в `Counterweight.Trebuchet/Interactables/`:
  - `WinchInteractable` — натянуть лебёдку (Idle → WindingUp).
  - `LoadStoneInteractable` — зарядить камень (Armed → Loaded).
  - `ReleaseLeverInteractable` — дёрнуть рычаг (Loaded → Firing).
- **Новое состояние `Loaded`** в `TrebuchetState`.
- **`TrebuchetController` отрефакторен**: вместо одного `RequestFire()` теперь три метода (`BeginWindUp`, `LoadProjectile`, `ReleaseShot`). Подписка на `InputBridge.FirePressed` убрана — клик по LMB больше **не запускает** требушет.

## Шаг 16. Создать Player prefab

### 16a. Новый GameObject
1. Открой **`FiringRange.unity`** (или любую сцену).
2. **GameObject → 3D Object → Capsule**. Это будет временное визуальное тело.
3. Переименуй в `Player`.
4. Удали с него **`Capsule Collider`** — нам нужен только `CharacterController`.
5. **Add Component → Character Controller**. Параметры:
   - **Slope Limit**: 45.
   - **Step Offset**: 0.3.
   - **Skin Width**: 0.08.
   - **Center**: (0, 0.9, 0).
   - **Radius**: 0.4.
   - **Height**: 1.8.
6. **Add Component → Player Controller** (`Counterweight.Player.PlayerController`).

### 16b. Камера-голова
1. Создай child: правый клик по `Player` → **Create Empty** → имя `Head`.
2. Position в локальных координатах: `(0, 1.65, 0)` (высота глаз).
3. Внутри Head создай child: **Camera** (правый клик → Camera, или **Create Empty** + Add Component → Camera). Имя `PlayerCamera`.
4. Камера в localPosition `(0, 0, 0)`, localRotation `(0, 0, 0)`.
5. **Удали Audio Listener** с этой камеры если есть лишний (оставь один на весь сцену).
6. **Tag** камеры: `MainCamera` (если в сцене ещё была старая Main Camera — её удали или сними тег).
7. На `PlayerCamera` **Add Component → Player Interactor** (`Counterweight.Player.PlayerInteractor`).
8. На `PlayerCamera` параметр **Ray Origin** — пусто оставь (скрипт сам подставит свой Transform).
9. **Max Distance**: 3 (метра).

### 16c. Ссылка на Camera Pivot в PlayerController
1. Выдели `Player`.
2. В `PlayerController` поле **Camera Pivot** перетащи `Head` (НЕ `PlayerCamera`).
3. Это нужно, чтобы pitch (вертикальный поворот) шёл по Head, а yaw (горизонтальный) по корню `Player`.

### 16d. Сохранить как prefab (опционально)
1. Перетащи `Player` из Hierarchy в `Assets/Prefabs/Player.prefab` для удобства.

## Шаг 17. Расставить интерактивные точки на префабе требушета

Открой `Assets/Prefabs/Trebuchet.prefab` (двойной клик).

Для **каждой** из трёх точек (Winch, LoadStone, ReleaseLever) сделай:

### 17a. Winch (лебёдка)
1. Внутри префаба создай child: **Create Empty** → `WinchPoint`.
2. Положение: где-то у основания требушета, рядом с физической лебёдкой / воротом. Если на модели нет явной лебёдки — поставь сбоку у основания.
3. **Add Component → Sphere Collider**:
   - **Is Trigger**: ✅ ON (чтобы игрок проходил сквозь, но raycast мог зацепиться).
   - **Radius**: 0.5.
4. **Add Component → Winch Interactable**.
5. **Controller**: перетащи корень требушета.

### 17b. LoadStone (корзина для камня)
1. Create Empty → `LoadStonePoint`.
2. Position: примерно где `ReleasePoint` или на корзине пращи — то место, куда «кладут» камень. Можно ниже, в зоне доступа игроку — например на уровне пояса возле стрелы.
3. **Sphere Collider** с Is Trigger ✅ и Radius 0.5.
4. **Add Component → Load Stone Interactable**.
5. **Controller**: корень требушета.

### 17c. ReleaseLever (рычаг)
1. Create Empty → `ReleaseLeverPoint`.
2. Position: где-нибудь, имитирующее рычаг — обычно с другой стороны от лебёдки.
3. **Sphere Collider** с Is Trigger ✅ и Radius 0.5.
4. **Add Component → Release Lever Interactable**.
5. **Controller**: корень требушета.

⚠️ Важно: **все три collider'а должны быть с Is Trigger = ON**. PlayerInteractor по умолчанию использует `QueryTriggerInteraction.Collide` для raycast'а, так что триггеры он находит.

⚠️ Если не уверен в позициях — пока что просто разнеси три точки вокруг основания требушета на расстоянии 1-2 метра друг от друга. Главное — чтобы можно было подойти к каждой и навестись курсором.

Сохрани префаб.

## Шаг 18. Обновить FiringRange сцену

1. Открой `FiringRange.unity`.
2. **Удали** старую Main Camera (с `DebugFlyCamera`) — её заменяет камера в Player prefab.
3. Размести `Player` GameObject на земле в пределах ~5-10 метров от требушета. Например `(5, 0, -5)`.
4. **`Input` GameObject** с `InputBridge` можно **удалить** — его никто не слушает теперь. (Или оставить, не мешает.)
5. Сохрани сцену.

## Шаг 19. Smoke test Iteration 3

1. Play.
2. Курсор должен исчезнуть, мышь крутит вид.
3. **WASD** ходит, **Shift** ускоряет.
4. **Esc** — курсор возвращается. **LMB** в окне Game — снова захватывается.
5. Подойди к требушету. Наведись на одну из трёх точек:
   - На лебёдке — внизу появится «**[E] Натянуть лебёдку**».
   - На корзине — пусто (state Idle, эта интеракция недоступна).
6. Нажми **E** на лебёдке. Запустится WindUp анимация.
7. Через секунду состояние станет Armed. Подойди к корзине → «**[E] Зарядить камень**». Нажми E.
8. Подойди к рычагу → «**[E] Дёрнуть рычаг**». Нажми E. Снаряд летит.
9. Через 3 секунды requirements возвращаются в Idle, цикл сначала.
10. Призрак траектории видно в Idle/Armed/Loaded, нет в Firing/Released.

### 19a. Если что-то не работает

| Симптом | Причина |
|---|---|
| Курсор не захватывается | PlayerController не на корне Player; CharacterController отсутствует |
| Камера не крутится | Camera Pivot в PlayerController не назначен на Head |
| Не появляется prompt при наведении | Collider на интерактивной точке отключён, не Trigger, или интерактив скрипт без ссылки на controller |
| Промпт «недоступен» (state неподходящий) | По дизайну: лебёдка только в Idle, корзина только в Armed, рычаг только в Loaded. CanInteract проверяет состояние |
| Раз нажал E — несколько раз сработало | Не должно — `wasPressedThisFrame` фиксирует только один кадр. Если повторяется — проверь, что нет двух PlayerInteractor'ов |
| Игрок проваливается под землю | Plane коллайдер односторонний; используй Cube с scale (20, 0.5, 20) или включи `Convex` на MeshCollider |
| Игрок не двигается | CharacterController застрял в геометрии. Поставь Player выше уровня земли; Skin Width не должен быть 0 |
| Crosshair не виден | OnGUI рендерит поверх — проверь, что не закрыт чёрной зоной (например, dropdown'ом). Должен быть 4×4 белый пиксель в центре |

## Шаг 20. (Опционально) Удалить ненужные старые компоненты

Теперь, когда Player работает:
- Из сцены `FiringRange` можно убрать `Input` GameObject (с `InputBridge`).
- Скрипт `DebugFlyCamera.cs` остался в `Counterweight.Trebuchet/`, но не используется. Можно удалить — игра без него не сломается.
- `InputBridge.cs` не вызывается ничем — можно тоже удалить вместе с самой сборкой `Counterweight.Input`. Но безопаснее оставить пока: возможно понадобится для других input-фичей позже.

---

## Управление в Iteration 3

| Клавиша | Действие |
|---|---|
| WASD | Ходьба |
| Shift | Спринт |
| Mouse | Обзор |
| E | Интеракция |
| ←/→ | Поворот основания требушета (через TrebuchetAimController) |
| ↑/↓ или Mouse Wheel | Регулировка мощности |
| Esc | Освободить курсор |
| LMB (в Game-окне) | Захватить курсор |

---

# Iteration 4 — Цели и разрушение

В коде уже есть:
- **`Counterweight.Demolition.asmdef`** — новая сборка.
  - `BlockConfig` — ScriptableObject с массой / HP / breakForce / damage knobs.
  - `Block` — Rigidbody + BoxCollider брикет, копит урон, при HP=0 рвёт свои joints.
  - `DemolitionTarget` — top-level контейнер, считает «стабильность» каждые 0.25с, эмитит события `StabilityChanged` / `DemolitionComplete`.
  - `TowerBuilder` — редакторный грид-билдер (правый клик → Build Tower).
  - `DemolitionHUD` — OnGUI отображение стабильности.

## Шаг 21. Создать `BlockConfig` ассет

1. Правый клик в `Assets/ScriptableObjects` → **Create → Counterweight → Block Config**.
2. Имя: `BlockConfig_Stone`.
3. Параметры по умолчанию:
   - **Mass**: 50.
   - **Max HP**: 30.
   - **Damage Threshold**: 30.
   - **Damage Multiplier**: 0.3.
   - **Joint Break Force**: 1500.
   - **Joint Break Torque**: 1500.
4. Эти числа подобраны под дефолтный требушет (~33 м/с, 8 кг снаряд). Тюнить будем после смок-теста.

## Шаг 22. Собрать `Block` prefab

1. **GameObject → 3D Object → Cube**. Имя `Block`.
2. Position `(0, 0, 0)`, Scale `(1, 1, 1)` (`TowerBuilder` сам выставит scale при спавне).
3. На Cube есть `BoxCollider` и `MeshRenderer` — оставляем.
4. **Add Component → Rigidbody**:
   - **Mass**: 50 (всё равно перезапишется из конфига).
   - **Use Gravity**: ✅.
   - **Is Kinematic**: ❌.
   - **Collision Detection**: **Continuous** (важно — чтобы быстрые снаряды попадали).
5. **Add Component → Block** (`Counterweight.Demolition.Block`).
6. **Block Config**: перетащи `BlockConfig_Stone`.
7. (Опционально) Поставь нейтральный серый Material'е для каменного вида.
8. Перетащи Cube из Hierarchy в `Assets/Prefabs/Block.prefab`. Удали из сцены.

## Шаг 23. Построить тестовую башню

### 23a. Контейнер
1. В сцене `FiringRange.unity`:
2. **GameObject → Create Empty** → имя `Tower`.
3. Position: например `(0, 0, 30)` — 30 метров перед требушетом.
4. **Add Component → Tower Builder**:
   - **Block Prefab**: перетащи `Block.prefab`.
   - **Block Config**: `BlockConfig_Stone`.
   - **Block Size**: 1.
   - **Width**: 2.
   - **Depth**: 2.
   - **Layers**: 15.
   - **Base Height Offset**: 0.

### 23b. Сборка
1. В Inspector у `Tower Builder` нажми **три точки** в правом верхнем углу компонента → **Build Tower**.
2. В Hierarchy внутри `Tower` появятся 60 child-объектов `Block_X_Z_LY`.
3. В Scene view — башня 2×2×15.

⚠️ Если что-то не так — нажми **Clear** (там же в три точки), поправь параметры, нажми **Build Tower** снова.

⚠️ Башня из 60 блоков с joints может секунду-две **выглядеть кривой** в Edit mode — joints стабилизируются только при запуске Play. Это норма.

### 23c. DemolitionTarget + HUD
1. На `Tower` GameObject **Add Component → Demolition Target**:
   - **Completion Threshold**: 0.2 (победа при ≤ 20% стабильности).
   - **Standing Tolerance**: 0.5 (метр от стартовой позы).
   - **Poll Interval**: 0.25.
2. **Add Component → Demolition HUD**:
   - **Target**: перетащи сам `Tower` GameObject (или `DemolitionTarget` компонент).
   - **Label**: "Стабильность" (или своё).
   - **Screen Offset**: (20, 20).

## Шаг 24. Smoke test

1. Play.
2. Видишь башню вдали и в углу экрана **«Стабильность: 100%»**.
3. Подойди к требушету, цикл интеракций как раньше: **E** на лебёдке → **E** на корзине → подкрути мощность под башню (стрелочки/скролл) → прицелься (стрелочки) → **E** на рычаге.
4. Камень летит в башню. При попадании:
   - Если удар сильный — joints трескаются с характерным «*chunk*», блоки разлетаются.
   - Если удар слабый — блок просто получает урон, башня стоит.
5. После пары удачных попаданий башня начинает крениться, верхушка обрушивается.
6. HUD показывает уменьшающуюся стабильность.
7. Когда стабильность падает ≤ 20% — HUD меняется на **«Стабильность: 0% — Демонтаж завершён»**.

### 24a. Тюнинг

| Симптом | Что покрутить |
|---|---|
| Прямое попадание не разрушает joint | Понизить `Joint Break Force` (1500 → 800) или повысить мощность требушета |
| Башня сама разваливается на старте | Joint Break Force/Torque слишком низкий или Block Config Mass слишком большой; либо blocks физически пересекаются — проверь `Block Size = 1` и единичный scale |
| Снаряд ударяет, но почти ничего не происходит | Понизь `Damage Threshold` (30 → 10) или подними `Damage Multiplier` (0.3 → 0.6) |
| Блоки слишком лёгкие, разлетаются как пенопласт | Подними `Mass` (50 → 100). Балансируй с `breakForce` |
| Башня стоит «как влитая» | Убедись что Joints с `enableCollision = false` не пересекаются и нет clipping; возможно блоки утонули в плоскость — сдвинь `Tower` вверх на 0.5 |
| HUD не обновляется | DemolitionTarget поллит каждые 0.25с — подожди. Если совсем нет — проверь, что `Target` назначен в HUD |
| Кадры падают | Активные Rigidbodies; включи **Auto-sleep** (по умолчанию включено) или подожди — упавшие блоки засыпают |

### 24b. Физика стэка — почему первые значения были плохие

**Проблема:** дефолтные `mass=50` и `breakForce=1500` физически несовместимы для башни в 12+ слоёв.

```
Нагрузка на нижний joint = N_layers × mass × g
                         = 11 × 50 × 9.81 ≈ 5400 N
breakForce 1500 << 5400  →  joint мгновенно рвётся
```

Башня складывается под собственным весом ещё до первого выстрела.
В 3×3 башне больше горизонтальных joint'ов распределяют вес, поэтому она «стоит»,
но и снаряд распределяет удар — ничего не пробивается.

**Решение:** блоки в 10 раз легче (mass=5), плюс higher solver iterations.

```
Новая нагрузка = 11 × 5 × 9.81 ≈ 540 N
breakForce 1200 → запас прочности ~2.2x  → стек стабилен
Импульс снаряда (8 кг × 33 м/с) ≈ 200 N·s за 0.05с ≈ 4000 N
                                                  → joint рвётся с одного попадания
```

### 24c. Что обновить в существующих ассетах

Я уже изменил **дефолты** в `BlockConfig.cs`, но твой существующий
`BlockConfig_Stone.asset` хранит старые значения. Открой его в Inspector и поставь:

| Поле | Было | Стало |
|---|---|---|
| Mass | 50 | **5** |
| Max HP | 30 | **20** |
| Damage Threshold | 30 | **20** |
| Damage Multiplier | 0.3 | 0.3 |
| Joint Break Force | 1500 | **1200** |
| Joint Break Torque | 1500 | **1200** |

Затем:
1. Выдели `Tower` GameObject в сцене.
2. На `TowerBuilder` нажми ⋮ → **Clear**.
3. Затем ⋮ → **Build Tower**.
Башня пересоберётся с новыми параметрами.

⚠️ **Важно**: на префабе `Block.prefab` поле Mass у `Rigidbody` тоже стояло 50 — но это
не страшно, скрипт `Block.Awake()` перезапишет его из `BlockConfig`. Можно
ничего не менять в префабе.

### 24d. Параметры баланса (после фикса)

С дефолтным требушетом (200кг контрвес, 0.55 efficiency, 8кг снаряд) скорость снаряда ≈ 32.85 м/с.
Импульс при ударе ≈ 100–200 N·s.

При `damageThreshold = 20, damageMultiplier = 0.3`:
- Прямое попадание: ~54 урона → **выбивает блок с одного раза** (HP=20).
- Касание/скользящий удар: ~9 урона → нужно 3 попадания.

При `breakForce = 1200`:
- Прямое лобовое — joints рвутся мгновенно, блок и его соседи отлетают.
- Удары в середину башни — рвут локальные joints, верх обрушивается под собственным весом → катарсис.

### 24e. Дальнейший тюнинг

| Хочу | Поверни |
|---|---|
| Башня всё равно складывается сама | mass↓ (3) или breakForce↑ (2000) |
| Снаряд почти не пробивает | breakForce↓ (800) или мощность требушета↑ |
| Хочу более прочную «гранитную» башню | breakForce↑ + maxHp↑ + одновременно подними мощность требушета (`launchEfficiency` или power-multiplier) |
| Лёгкие глиняные стены | mass=2, maxHp=10, breakForce=600 |

---

## Управление в Iteration 4

| Клавиша | Действие |
|---|---|
| WASD / Mouse / E | Ходьба, обзор, интеракция (как было) |
| ←/→ | Поворот основания требушета |
| ↑/↓ или Mouse Wheel | Регулировка мощности |
| Esc / LMB | Курсор |

---

# Iteration 5 — Полировка катарсиса

В коде уже есть:
- **Block** теперь эмитит события `Damaged(damage, point)` и `Detached(point)`.
- **`DemolitionEffectsConfig`** — SO, держит ссылки на VFX/SFX/shake-параметры (любое поле может быть `null`, эффект просто не сыграет).
- **`BlockEffects`** — на каждом блоке слушает `Block`, спавнит частицы и звуки, дёргает CameraShake.
- **`CameraShake`** — Perlin-noise шейк на localPosition, статический `AddImpact(intensity)`.
- **`DemolitionRumble`** — следит за `DemolitionTarget.StabilityChanged`, на больших обвалах включает rumble + большой shake.
- **`BlockDebrisCleaner`** — упавшие отделённые блоки уничтожаются после ~30 сек сна, или сразу если ушли ниже `voidY`.

## Шаг 25. Создать частицы пыли

### 25a. ImpactDust prefab (попадание снаряда)
1. **GameObject → Effects → Particle System**. Имя `ImpactDust`.
2. В Inspector у Particle System настрой главный модуль:
   - **Duration**: 0.5
   - **Looping**: ❌
   - **Start Lifetime**: random between 0.5 and 1.0
   - **Start Speed**: random between 1 and 3
   - **Start Size**: random between 0.1 and 0.3
   - **Start Color**: серо-бежевый (например RGBA `200, 190, 170, 200`)
   - **Gravity Modifier**: 0.5 (пыль слегка оседает)
3. Раздел **Emission**:
   - **Rate over Time**: 0
   - **Bursts**: добавь один → Time 0, Count 15-25.
4. Раздел **Shape**:
   - **Shape**: Sphere
   - **Radius**: 0.1
5. **Size over Lifetime**: ✅ → curve от 1 в 0 (затухает).
6. **Color over Lifetime**: ✅ → alpha по timeline от ~200 до 0.
7. **Renderer → Material**: создай новый материал `M_DustParticle` с шейдером **Universal Render Pipeline → Particles → Unlit**, **Surface Type = Transparent**, **Blending Mode = Alpha**, **Base Map = Default-Particle** (стандартная белая текстура частицы из Built-in).
   ⚠️ Без URP-совместимого материала пыль будет рендериться розовой/чёрной.
8. Перетащи `ImpactDust` из Hierarchy в `Assets/Prefabs/VFX/ImpactDust.prefab`. Удали из сцены.

### 25b. DetachDust prefab (срыв блока)
Скопируй `ImpactDust.prefab` (Ctrl+D в Project), переименуй в `DetachDust`. Потом открой и подкрути:
- **Duration**: 0.8
- **Bursts**: Count 40-60
- **Start Speed**: random 2–5
- **Start Size**: random 0.15–0.4
- **Shape Radius**: 0.2
- **Gravity Modifier**: 0.2 (пыль чуть дольше висит)

Это даст более «густое» облако для срыва.

## Шаг 26. Найти/создать аудиоклипы

Тебе нужны 3 коротких WAV/OGG файла:
- **Thud** — 0.2-0.4 сек, глухой удар камня (попадание снаряда).
- **Crack** — 0.3-0.6 сек, треск/скол (срыв joint'а).
- **Rumble** — 1-3 сек, низкий грохот (обвал больших масс).

Где взять:
- **freesound.org** (бесплатно, требует регистрации) — поиск: `stone impact`, `rock crack`, `rubble collapse`. Лицензии CC0 или CC-BY.
- **Sonniss GDC** — бесплатные паки для геймдева (sonniss.com/gameaudiogdc).
- **Microsoft Sound** или сгенерируй через Audacity.

Сохрани в `Assets/Audio/Demolition/`. Unity автоматически импортирует.

⚠️ Если пока нет — оставь поля в конфиге пустыми, всё остальное (частицы, шейк) работает независимо.

## Шаг 27. Создать `DemolitionEffectsConfig` ассет

1. Правый клик в `Assets/ScriptableObjects` → **Create → Counterweight → Demolition Effects Config**.
2. Имя: `DemolitionEffects_Stone`.
3. Заполни:
   - **Impact Dust Prefab** = `ImpactDust.prefab`
   - **Detach Dust Prefab** = `DetachDust.prefab`
   - **Impact Dust Lifetime**: 2
   - **Detach Dust Lifetime**: 4
   - **Impact Thud Clip** = (твой thud .wav)
   - **Impact Thud Volume**: 0.4
   - **Joint Crack Clip** = (твой crack .wav)
   - **Joint Crack Volume**: 0.6
   - **Collapse Rumble Clip** = (твой rumble .wav)
   - **Collapse Rumble Volume**: 0.7
   - **Impact Shake**: 0.0 (на одиночный удар не трясём — раздражает)
   - **Detach Shake**: 0.04
   - **Collapse Shake Per Percent**: 0.01
   - **Collapse Shake Min**: 0.05
   - **Collapse Threshold**: 0.04 (4% drop в одном poll = collapse)

## Шаг 28. Обновить `Block` prefab

1. Открой `Assets/Prefabs/Block.prefab` (двойной клик).
2. **Add Component → Block Effects** → поле **Config**: `DemolitionEffects_Stone`.
3. **Add Component → Block Debris Cleaner**:
   - **Rest Cleanup Delay**: 30 (секунд после засыпания → уничтожение).
   - **Void Y**: -25.
4. Сохрани.

⚠️ Обновлённый префаб **автоматически** применится ко всем блокам в башне. **Не нужно** пересобирать `Tower` через Build Tower (но можно, чтобы убедиться).

## Шаг 29. Подключить `DemolitionRumble` к башне

1. Выдели `Tower` в сцене.
2. **Add Component → Demolition Rumble** → поле **Config**: `DemolitionEffects_Stone`.

## Шаг 30. Подключить `CameraShake` к голове игрока

1. Открой `Player` prefab или просто инстанс в сцене.
2. На GameObject **`Head`** (родитель PlayerCamera) **Add Component → Camera Shake**.
3. Параметры по умолчанию ок:
   - **Decay**: 4
   - **Max Offset**: 0.25
   - **Frequency**: 22.

⚠️ **Не вешать на сам PlayerCamera**, иначе при нажатии CameraShake перепишет позицию, которую может попробовать менять что-то другое позже. На Head — родителе камеры — безопасно.

## Шаг 31. Smoke test Iteration 5

1. Play.
2. Подойди к требушету, натяни → заряди → стреляй в башню.
3. **Что должно быть:**
   - При попадании снаряда в блок — облако пыли + thud-звук.
   - При срыве блока с joint'ов — большое облако пыли + crack-звук + лёгкая тряска камеры.
   - При обвале нескольких блоков сразу (>4% массы за один poll) — rumble-звук + сильная тряска.
   - Упавшие блоки лежат секунд 30, потом исчезают (производительность).
   - HUD стабильности падает как раньше.

### 31a. Если что-то не так

| Симптом | Причина |
|---|---|
| Пыль рендерится розовой | Материал на Particle System не URP-совместимый. Поставь Particles/Unlit URP. |
| Звуки не играют | `AudioSource.PlayClipAtPoint` — нужен AudioListener в сцене (на PlayerCamera обычно есть автоматически). Проверь. |
| Камера трясётся бесконечно | `currentIntensity` не обнуляется — проверь, что `decay > 0` в CameraShake. |
| Камера трясётся, но как будто пьяная | `frequency` слишком низкий → медленный сильный сдвиг. Подними до 25-30. |
| Шейк ломает обзор камеры | CameraShake висит на Head, а PlayerController крутит Head по rotation. Должно работать, но если конфликтует — попробуй вынести в child Head GO. |
| Блоки не уничтожаются после падения | Block.IsDetached=false (joints всё ещё держат). Нормально — значит блок не упал. |
| Блоки уничтожаются прямо во время битвы | `Rest Cleanup Delay` слишком короткий, или блок в покое уже был. Подними до 60. |
| Слишком много частиц / лагает | Уменьши Bursts.Count в обоих prefab'ах. |

### 31b. Тонкий тюнинг по вкусу

- **Меньше пыли при попадании** — снизь Burst Count в `ImpactDust` до 8-10.
- **Драматичнее обвал** — повысь `Collapse Shake Per Percent` до 0.02 и `Collapse Shake Min` до 0.1.
- **Тише шейк** — снизь `Detach Shake` до 0.02 и `Collapse Shake Min` до 0.03. Помни про cozy: дрожать должно **слегка**, не как в шутерах.
- **Эстетика звука** — для cozy лучше глухие, древние, не «современный экшен» сэмплы. Пыльный «*hpf*» вместо стерильного thud.

---

## Управление в Iteration 5

То же что в Iteration 4. Эффекты — пассивные, реагируют на разрушение.

---

# Iteration 6 — Engineer's Lens + типы снарядов

В коде уже есть:

**Типы снарядов:**
- `BallisticsSolver` / `BallisticsSimulator` — `projectileMass` теперь **обязательный параметр**, а не поле в TrebuchetConfig.
- `TrebuchetConfig.projectileMass` — **удалён** (масса берётся из выбранного `ProjectileConfig`).
- `ProjectileSelector` — список доступных типов, переключение по 1/2/3, lock на состоянии Loaded.
- `ProjectileSelectorHUD` — список снарядов в правом верхнем углу, активный отмечен ●.
- `LoadStoneInteractable` — забирает `selector.Current` в момент загрузки → `controller.LoadProjectile(config)`.
- `TrebuchetController.LoadedProjectile` — то, что физически в корзине. При Fire выстреливается именно оно.
- `TrajectoryRenderer` — призрак считает с массой/drag активного снаряда.

**Engineer's Lens:**
- `EngineerLens` (static) — глобальное состояние + `ActiveChanged` event.
- `EngineerLensInput` — слушает Tab, тоггл состояния, маленькая HUD-подсказка слева внизу.
- `BlockLensIndicator` — per-block: при активной лупе красит блок через `MaterialPropertyBlock` lerp(damaged → healthy) по HpFraction.
- `Block.MaxHp` / `HpFraction` — публичные accessors.

## Шаг 32. Обновить `TrebuchetConfig_Default`

Поле **Projectile Mass** в Inspector **исчезло** (удалил из кода). Asset просто потерял эту переменную — ничего страшного, остальные поля целы.

⚠️ Если у тебя вылетают **CompilerError'ы** про `projectileMass` — обнови Unity (Ctrl+R пересборка). Все ссылки в коде заменены.

## Шаг 33. Создать три ProjectileConfig'а

### 33a. Stone (current — переименуй существующий)
Открой `Assets/ScriptableObjects/ProjectileConfig_Stone`. Уже есть. Параметры:
- **Mass**: 8
- **Linear Damping**: 0.05
- **Angular Damping**: 0.05
- **Prefab**: `StoneProjectile.prefab`

### 33b. ClayPot (новый)
1. Правый клик в `ScriptableObjects` → **Create → Counterweight → Projectile Config**.
2. Имя: `ProjectileConfig_ClayPot`.
3. Параметры:
   - **Mass**: 3
   - **Linear Damping**: 0.1 (более «парусный»)
   - **Angular Damping**: 0.1
   - **Prefab**: пока тот же `StoneProjectile` (или сделай копию ниже).

### 33c. WoodenBall (новый)
1. **Create → Counterweight → Projectile Config**.
2. Имя: `ProjectileConfig_WoodenBall`.
3. Параметры:
   - **Mass**: 1.5
   - **Linear Damping**: 0.05
   - **Angular Damping**: 0.05
   - **Prefab**: `StoneProjectile` или копия с другим цветом.

### 33d. (Опционально) Визуальные prefab'ы
Чтобы три снаряда выглядели по-разному:
1. Скопируй `StoneProjectile.prefab` → переименуй в `ClayPotProjectile`. Покрась materialом светло-коричневого глиняного цвета.
2. Скопируй ещё раз → `WoodenBallProjectile`. Тёмно-коричневый.
3. В `ProjectileConfig_ClayPot.Prefab` поставь `ClayPotProjectile`, в `WoodenBall` соответственно.

⚠️ Можешь это пропустить — функционально работает с одним префабом.

## Шаг 34. Обновить `Trebuchet.prefab`

1. Открой `Assets/Prefabs/Trebuchet.prefab`.
2. На корневом GameObject **Add Component → Projectile Selector**:
   - **Configs**: добавь три элемента: `Stone`, `ClayPot`, `WoodenBall` (в этом порядке — индексы 1/2/3).
   - **Lock Source**: перетащи `TrebuchetController` (тот же GameObject).
3. На корневом GameObject найди **`LoadStoneInteractable`** (тот, что внутри `LoadStonePoint`):
   - **Selector**: перетащи `ProjectileSelector` (компонент с корня требушета).
4. На `TrajectoryGhost` GameObject в `TrajectoryRenderer`:
   - **Projectile Selector**: перетащи Selector с корня.
   - **Fallback Projectile Config**: оставь `ProjectileConfig_Stone` (на случай, если Selector пустой).
5. На `ProjectileSpawner` в поле **Default Projectile Config** поставь `ProjectileConfig_Stone` (fallback).

## Шаг 35. HUD выбора снаряда

1. Создай в сцене пустой GameObject `ProjectileHUD` (или прямо на `Player`).
2. **Add Component → Projectile Selector HUD**:
   - **Selector**: перетащи Selector с требушета.
   - **Screen Offset**: (20, 20).

🔍 Запусти Play. В правом верхнем углу должен появиться список:
```
[1] ProjectileConfig_Stone  ●
[2] ProjectileConfig_ClayPot
[3] ProjectileConfig_WoodenBall
```

Нажми **2** → активный сместится. Призрак траектории должен немедленно перерисоваться (короткая дуга для лёгкого снаряда → дальний полёт).

## Шаг 36. Engineer's Lens

### 36a. Подключить input
1. На сцене (или на `Player`) **Add Component → Engineer Lens Input**. Параметры по умолчанию ок.

### 36b. Подключить per-block indicator
1. Открой `Assets/Prefabs/Block.prefab`.
2. **Add Component → Block Lens Indicator**. Параметры:
   - **Healthy Color**: светло-зелёный (по умолчанию).
   - **Damaged Color**: оранжево-красный.
3. Сохрани префаб. **Все блоки в башне получат этот компонент автоматически** (через инстансиро через TowerBuilder или через prefab override).

⚠️ Если ты собрал башню **до** добавления BlockLensIndicator на префаб — пересобери: на `Tower` GameObject → ⋮ → Clear → ⋮ → Build Tower.

### 36c. Smoke test лупы
1. Play. Нажми **Tab** — внизу левого угла подсказка переключится на «вкл».
2. Все блоки в башне должны окраситься в **зелёный**.
3. Стреляй — попадаешь, блок получает урон, **меняет цвет от зелёного к оранжевому-красному**.
4. Когда блок отрывается (Detached), окраска **снимается** — ты видишь, что он больше не часть структуры.
5. Нажми Tab снова — все цвета сбрасываются.

⚠️ Если блоки розовые при включённой лупе — это URP проблема: твой материал на блоке не использует `_BaseColor`. Стандартный URP/Lit всегда использует это имя. Если у тебя custom shader, ищи имя свойства цвета и поправь `BaseColorId` в коде. Или проще: поставь блокам стандартный URP/Lit материал.

## Шаг 37. Smoke test полного Iteration 6

1. Play.
2. В правом верхнем углу — список снарядов, активный Stone.
3. Призрак траектории — стандартная дуга.
4. Нажми **2** → Clay Pot. Призрак удлиняется (легче снаряд → летит быстрее → дальше).
5. Нажми **3** → Wooden Ball. Призрак ещё длиннее.
6. Нажми **Tab** → башня окрашивается зелёным.
7. Подойди к лебёдке → E. Подойди к корзине → E (загрузка снаряда — текущий выбранный заряжается). Заметь: **HUD списка теперь не реагирует на 1/2/3** — заблокирован, пока не выстрелишь.
8. Подойди к рычагу → E.
9. Wooden Ball улетает дальше Stone, но при попадании наносит **меньше урона** (меньше масса → меньше импульс → меньше damage).
10. После Released можно снова менять снаряд.

### 37a. Тонкий тюнинг

| Хочу | Поверни |
|---|---|
| Stone слишком слабый | `Damage Threshold` ↓ или `Damage Multiplier` ↑ в `BlockConfig_Stone` |
| Wooden Ball вообще не наносит урон | в `BlockConfig` понизь `Damage Threshold` (10 вместо 20). Лёгкий снаряд имеет малый импульс — порог его «съедает» |
| Clay Pot и Wooden Ball выглядят одинаково | сделай разные prefab'ы (шаг 33d) с разными цветами/размерами |
| Лупа цвета слишком ярко | подкрути `Healthy Color`/`Damaged Color` в `BlockLensIndicator` (уменьши saturation) |
| Призрак не обновляется при смене снаряда | `Trajectory Renderer.Projectile Selector` не назначен. Проверь |

---

## Управление в Iteration 6

| Клавиша | Действие |
|---|---|
| WASD / Mouse / E | Ходьба, обзор, интеракция |
| ←/→ | Поворот основания |
| ↑/↓ или Mouse Wheel | Регулировка мощности |
| **1 / 2 / 3** | Выбор типа снаряда |
| **Tab** | Лупа инженера (вкл/выкл) |
| Esc / LMB | Курсор |

---

## Что **не нужно** делать

- Не трогать `SampleScene.unity` — там пусто, она нам не интересна.
- Не править `TrebuchetController.cs`, `TrebuchetAnimationRelay.cs` и т.п. вручную —
  если что-то нужно поменять, пиши мне и я подправлю код.
- Не добавлять второй `InputBridge` — он singleton.
- Не трогать `git status` файлы `Assets/_3Dmodels/Trebuchet/Trebuchet_LODG.prefab*`
  если они есть — это то, что у тебя уже было. Если хочешь — можешь их
  удалить и сделать чистый `Trebuchet.prefab` по этой инструкции.
