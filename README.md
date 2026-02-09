# 生物模拟实验

## 数据

### 实验器材

定义如试管、移液枪、灭菌箱之类的;
以及各种试剂(提供一个check类型,定义为workflow)

### Step

实现一个 Step 用于编辑每个步骤,包含当前的步骤所需要的所有实验仪器

## Detector

监听所有操作,在每一个操作后对里面的对应channel进行广播
比如移液结束通知移液频道, 并且对于不当操作进行 Hint 或者警告处理

在操作执行之后, 根据类型广播到checker上,checker会查询currentCondition
