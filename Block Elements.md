## Block Elements

### Paragraph

一行文字加一个以上的空行。



### Headers

```
^(#{1,6})\s+(.*)$
```



### Blockquotes

```
^\>\s+(.*)$
```



### Task List

```
^(- \[[ x]\])\s+(.*)$
```



### Lists

```
^[-*]\s+(.*)$ 
```



### (Fenced) Code Blocks

```
^```(?<lang>.*)$
```

### Math Blocks

```
^\$\$

```



### Tables

| Left-Aligned  | Center Aligned  | Right Aligned |
| :------------ | :-------------: | ------------: |
| col 3 is      | some wordy text |         $1600 |
| col 2 is      |    centered     |           $12 |
| zebra stripes |    are neat     |            $1 |
||

竖线的数量要正确，解析为数组的数组。

第一排，第二排。

### Footnotes

You can create footnotes like this[^footnote].

[^footnote]: Here is the *text* of the **footnote**.

### Horizontal Rules

```
^\*{3,}|\-{3,}$
```



### YAML Front Matter



### Table of Contents (TOC)