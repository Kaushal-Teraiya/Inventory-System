@tool
extends EditorScript

func _run() -> void:
	var slot := PanelContainer.new()
	slot.name = "InventorySlot"

	var slot_content := Control.new()
	slot_content.name = "SlotContent"
	slot.add_child(slot_content)
	slot_content.owner = slot

	var icon := TextureRect.new()
	icon.name = "ItemIcon"
	icon.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	icon.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED
	slot_content.add_child(icon)
	icon.owner = slot

	var quantity := Label.new()
	quantity.name = "QuantityLabel"
	quantity.horizontal_alignment = HORIZONTAL_ALIGNMENT_RIGHT
	quantity.vertical_alignment = VERTICAL_ALIGNMENT_BOTTOM
	slot_content.add_child(quantity)
	quantity.owner = slot

	var slot_packed := PackedScene.new()
	slot_packed.pack(slot)
	ResourceSaver.save(slot_packed, "res://InventorySlot.tscn")

	var root := Control.new()
	root.name = "InventoryUI"
	root.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)

	var background := ColorRect.new()
	background.name = "Background"
	background.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	background.color = Color(0, 0, 0, 0.55)
	root.add_child(background)
	background.owner = root

	var panel := PanelContainer.new()
	panel.name = "MainPanel"
	panel.set_anchors_and_offsets_preset(Control.PRESET_CENTER)
	panel.custom_minimum_size = Vector2(760, 460)
	root.add_child(panel)
	panel.owner = root

	var margin := MarginContainer.new()
	margin.name = "MarginContainer"
	panel.add_child(margin)
	margin.owner = root

	var main := VBoxContainer.new()
	main.name = "MainLayout"
	margin.add_child(main)
	main.owner = root

	var header := HBoxContainer.new()
	header.name = "Header"
	main.add_child(header)
	header.owner = root

	var title := Label.new()
	title.name = "Title"
	title.text = "Inventory"
	title.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	header.add_child(title)
	title.owner = root

	var close_header := Button.new()
	close_header.name = "CloseButton"
	close_header.text = "X"
	header.add_child(close_header)
	close_header.owner = root

	var content := HBoxContainer.new()
	content.name = "Content"
	content.size_flags_vertical = Control.SIZE_EXPAND_FILL
	main.add_child(content)
	content.owner = root

	var section := VBoxContainer.new()
	section.name = "InventorySection"
	section.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	content.add_child(section)
	section.owner = root

	var capacity := Label.new()
	capacity.name = "CapacityLabel"
	capacity.text = "Inventory: 0 / 20"
	section.add_child(capacity)
	capacity.owner = root

	var grid := GridContainer.new()
	grid.name = "InventoryGrid"
	grid.columns = 5
	grid.size_flags_vertical = Control.SIZE_EXPAND_FILL
	section.add_child(grid)
	grid.owner = root

	var slot_scene := load("res://InventorySlot.tscn") as PackedScene
	for i in range(20):
		var instance := slot_scene.instantiate()
		instance.name = "Slot_%02d" % i
		instance.custom_minimum_size = Vector2(64, 64)
		grid.add_child(instance)
		instance.owner = root

	var details := PanelContainer.new()
	details.name = "ItemDetails"
	details.custom_minimum_size.x = 190
	content.add_child(details)
	details.owner = root

	var details_layout := VBoxContainer.new()
	details_layout.name = "DetailsLayout"
	details.add_child(details_layout)
	details_layout.owner = root

	var detail_icon := TextureRect.new()
	detail_icon.name = "DetailIcon"
	detail_icon.custom_minimum_size = Vector2(96, 96)
	detail_icon.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	detail_icon.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED
	details_layout.add_child(detail_icon)
	detail_icon.owner = root

	var item_name := Label.new()
	item_name.name = "ItemName"
	item_name.text = "Select an item"
	details_layout.add_child(item_name)
	item_name.owner = root

	var description := Label.new()
	description.name = "ItemDescription"
	description.text = "Item description"
	description.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	description.size_flags_vertical = Control.SIZE_EXPAND_FILL
	details_layout.add_child(description)
	description.owner = root

	var actions := HBoxContainer.new()
	actions.name = "ActionButtons"
	details_layout.add_child(actions)
	actions.owner = root

	var use_button := Button.new()
	use_button.name = "UseButton"
	use_button.text = "Use"
	actions.add_child(use_button)
	use_button.owner = root

	var drop_button := Button.new()
	drop_button.name = "DropButton"
	drop_button.text = "Drop"
	actions.add_child(drop_button)
	drop_button.owner = root

	var footer := HBoxContainer.new()
	footer.name = "Footer"
	main.add_child(footer)
	footer.owner = root

	var sort_button := Button.new()
	sort_button.name = "SortButton"
	sort_button.text = "Sort"
	footer.add_child(sort_button)
	sort_button.owner = root

	var close_footer := Button.new()
	close_footer.name = "CloseButtonFooter"
	close_footer.text = "Close"
	footer.add_child(close_footer)
	close_footer.owner = root

	var packed := PackedScene.new()
	packed.pack(root)
	var result := ResourceSaver.save(packed, "res://InventoryUI.tscn")
	print("InventoryUI.tscn save result: ", result)
	print("Created InventorySlot.tscn and InventoryUI.tscn")
